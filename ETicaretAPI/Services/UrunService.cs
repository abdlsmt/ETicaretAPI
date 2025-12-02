using ETicaretAPI.Models;
using ETicaretAPI.Models.DTOs;
using ETicaretAPI.Repositories;
using Microsoft.Extensions.Caching.Distributed; // Redis için
using System.Text.Json; // JSON çevirimi için

namespace ETicaretAPI.Services; // File-Scoped Namespace

// Primary Constructor: (IGenericRepository<Urun> _repository)
public class UrunService(IGenericRepository<Urun> _repository, IDistributedCache _cache) : IUrunService
{
    public async Task<List<Urun>> TumunuGetirAsync()
    {
        string cacheKey = "urunler_listesi";

        // 1. ADIM: Redis'e bak, veri var mı?
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            // Veri varsa, JSON'dan geri çevir ve döndür (Veritabanına gitmedik!)
            return JsonSerializer.Deserialize<List<Urun>>(cachedData)!;
        }

        // 2. ADIM: Veri yoksa, Veritabanından çek
        var urunler = await _repository.TumunuGetirAsync("Kategori");

        // 3. ADIM: Veriyi Redis'e kaydet (Cache'le)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // 10 Dakika sakla
        };

        // Listeyi JSON'a çevirip saklıyoruz
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(urunler), cacheOptions);

        return urunler;
    }

    public async Task EkleAsync(UrunEkleDTO dto)
    {
        var yeniUrun = new Urun
        {
            Ad = dto.Ad,
            Aciklama = dto.Aciklama,
            Fiyat = dto.Fiyat,
            StokAdedi = dto.StokAdedi,
            KategoriId = dto.KategoriId
        };

        await _repository.EkleAsync(yeniUrun);

        // ÖNEMLİ: Yeni ürün eklenince Cache bayatlar! Silmemiz lazım ki yeni liste oluşsun.
        await _cache.RemoveAsync("urunler_listesi");
    }
}