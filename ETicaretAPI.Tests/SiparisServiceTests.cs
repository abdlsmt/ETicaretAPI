using ETicaretAPI.Data;
using ETicaretAPI.Models;
using ETicaretAPI.Models.DTOs;
using ETicaretAPI.Repositories;
using ETicaretAPI.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; // DbContext seçenekleri için
using Moq;
using Xunit;

namespace ETicaretAPI.Tests;

public class SiparisServiceTests
{
    // Test edeceğimiz servis
    private readonly SiparisService _siparisService;

    // Sahte Bağımlılıklar (Mocks)
    private readonly Mock<IGenericRepository<Siparis>> _mockSiparisRepo;
    private readonly Mock<IGenericRepository<Urun>> _mockUrunRepo;
    private readonly Mock<IBackgroundJobClient> _mockJobClient;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;
    private readonly ETicaretContext _fakeContext; // Context'i mocklamak zordur, in-memory kullanacağız

    public SiparisServiceTests()
    {
        // 1. Mock'ları Hazırla
        _mockSiparisRepo = new Mock<IGenericRepository<Siparis>>();
        _mockUrunRepo = new Mock<IGenericRepository<Urun>>();
        _mockJobClient = new Mock<IBackgroundJobClient>();
        _mockHttpContext = new Mock<IHttpContextAccessor>();

        // 2. Fake DbContext Hazırla (Transaction hatası almamak için)
        // Bellek üzerinde çalışan geçici bir veritabanı ayarı
        var options = new DbContextOptionsBuilder<ETicaretContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
        _fakeContext = new ETicaretContext(options);

        // 3. Servisi Oluştur (Tüm sahte parçaları birleştir)
        _siparisService = new SiparisService(
            _mockSiparisRepo.Object,
            _mockUrunRepo.Object,
            _fakeContext,
            _mockJobClient.Object,
            _mockHttpContext.Object
        );
    }

    [Fact]
    public async Task SiparisOlustur_Sepette10danFazlaCesitVarsa_HataFirlatmali()
    {
        // --- ARRANGE (Hazırlık) ---
        // 11 tane ürün içeren bir sepet oluşturuyoruz
        var cokKalabalikSepet = new List<SepetKalemiDTO>();
        for (int i = 1; i <= 11; i++)
        {
            cokKalabalikSepet.Add(new SepetKalemiDTO(i, 1)); // Ürün ID: i, Adet: 1
        }

        var dto = new SiparisOlusturDTO(cokKalabalikSepet);

        // --- ACT & ASSERT (Eylem ve Doğrulama) ---
        // Bu metodu çağırdığımda, bir Exception fırlatmasını bekliyorum.
        var hata = await Assert.ThrowsAsync<Exception>(() => _siparisService.SiparisOlusturAsync(dto));

        // Hata mesajı beklediğim gibi mi?
        Assert.Contains("10", hata.Message);
    }
}