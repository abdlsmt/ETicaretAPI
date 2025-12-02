using ETicaretAPI.Data;
using ETicaretAPI.Models;
using ETicaretAPI.Repositories;
using ETicaretAPI.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;

// admin@eticaret.com Admin123!

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVİSLER (Konteyner'a Yükleme) ---

builder.Services.AddControllers();

// Swagger (Klasik Yöntem)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ETicaretAPI", Version = "v1" });

    // "Authorize" Butonunu Ekle
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lütfen Token'ı 'Bearer {token}' formatında giriniz.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Bütün metodlara kilit işareti koy
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }});
});

// DbContext
builder.Services.AddDbContext<ETicaretContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  REDIS ENTEGRASYONU
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Repository'ler (Generic)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Servisler (Business Logic)
builder.Services.AddScoped<IKategoriService, KategoriService>();
builder.Services.AddScoped<ETicaretAPI.Services.IUrunService, ETicaretAPI.Services.UrunService>();
builder.Services.AddScoped<ISiparisService, SiparisService>();
builder.Services.AddScoped<IRaporService, RaporService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Şifre Kuralları (Geliştirme aşamasında gevşek tutalım)
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;

    // Email benzersiz olmalı
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ETicaretContext>()
.AddDefaultTokenProviders();

// HANGFIRE SERVİSİ (Veritabanı Ayarı)
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        // 👇 BU AYAR ÇOK ÖNEMLİ: Tablolar yoksa otomatik oluştur!
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true,
        PrepareSchemaIfNecessary = true
    }));

builder.Services.AddHangfireServer();

// JWT AYARLARI
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"]
    };
});

var app = builder.Build();

// --- 2. KATMANLAR (Middleware) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// DASHBOARD (Panel) AÇILIMI
app.UseHangfireDashboard();

app.MapControllers(); // Controller rotalarını eşleştir

// VERİTABANI TOHUMLAMA (SEEDING) İŞLEMİ
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // Hata olursa uygulama çökmesin diye try-catch
    try
    {
        // Docker için Veritabanını otomatik oluşturur
        var context = services.GetRequiredService<ETicaretContext>();
        await context.Database.MigrateAsync();
        await ETicaretAPI.Data.DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Seeding sırasında hata: " + ex.Message);
    }
}

app.Run();