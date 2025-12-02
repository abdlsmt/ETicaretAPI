using ETicaretAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Data;

public static class DbSeeder
{
    // Bu metodu Program.cs'de çağıracağız
    public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
    {
        // Servisleri çağırıyoruz
        var userManager = service.GetRequiredService<UserManager<AppUser>>();
        var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

        // 1. Rolleri Oluştur (Yoksa)
        await RoleOlustur(roleManager, "Admin");
        await RoleOlustur(roleManager, "Customer");

        // 2. Default Admin Kullanıcısı Oluştur (Yoksa)
        var adminUser = await userManager.FindByEmailAsync("admin@eticaret.com");
        if (adminUser == null)
        {
            var user = new AppUser
            {
                UserName = "admin@eticaret.com",
                Email = "admin@eticaret.com",
                Ad = "Sistem",
                Soyad = "Yöneticisi",
                EmailConfirmed = true
            };

            // Şifre kurallarına uygun bir şifre
            await userManager.CreateAsync(user, "Admin123!");

            // Kullanıcıya Admin rolünü ata
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }

    private static async Task RoleOlustur(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}