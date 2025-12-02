using ETicaretAPI.Models;
using ETicaretAPI.Repositories;
using ETicaretAPI.Services;
using Moq; // Sahte nesne kütüphanesi
using Xunit; // Test motoru

namespace ETicaretAPI.Tests;

public class KategoriServiceTests
{
    // Test edeceğimiz servis
    private readonly KategoriService _kategoriService;

    // Sahte Depocu (Moch Repository)
    // Gerçek veritabanına gitmeyeceğiz, bu taklitçiyle çalışacağız.
    private readonly Mock<IGenericRepository<Kategori>> _mockRepo;

    public KategoriServiceTests()
    {
        // 1. Sahte depocuyu oluştur
        _mockRepo = new Mock<IGenericRepository<Kategori>>();

        // 2. Servisi oluştururken içine gerçek değil, sahte depocuyu ver
        _kategoriService = new KategoriService(_mockRepo.Object);
    }

    [Fact] // Bu bir test metodudur etiketi
    public async Task TumunuGetir_VeriVarsa_ListeDonmeli()
    {
        // ARRANGE (Hazırlık)
        // Sahte depocuya diyoruz ki: "Senden TumunuGetirAsync istenirse, şu sahte listeyi ver."
        var sahteListe = new List<Kategori>
        {
            new Kategori { Id = 1, Ad = "Elektronik" },
            new Kategori { Id = 2, Ad = "Giyim" }
        };

        _mockRepo.Setup(x => x.TumunuGetirAsync(null))
                 .ReturnsAsync(sahteListe);

        // ACT (Eylem)
        // Servisi çalıştırıyoruz
        var sonuc = await _kategoriService.TumunuGetirAsync();

        // ASSERT (Doğrulama)
        // Sonuç istediğimiz gibi mi?
        Assert.NotNull(sonuc); // Boş olmamalı
        Assert.Equal(2, sonuc.Count); // 2 tane gelmeli
        Assert.Equal("Elektronik", sonuc[0].Ad); // İlkinin adı Elektronik olmalı
    }
}