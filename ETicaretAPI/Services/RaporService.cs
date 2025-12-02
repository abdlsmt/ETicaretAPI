using ETicaretAPI.Data;
using ETicaretAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services;

public class RaporService(ETicaretContext _context) : IRaporService
{
    public async Task<DashboardRaporDTO> GenelRaporGetirAsync()
    {
        // 1. TOPLAM CİRO (Sum)
        // SQL Karşılığı: SELECT SUM(ToplamTutar) FROM Siparisler
        // Lambda (x => x...): "Her bir sipariş (x) için, git onun ToplamTutarını al ve topla."
        var ciro = await _context.Siparisler.SumAsync(x => x.ToplamTutar);

        // 2. TOPLAM SİPARİŞ (Count)
        // SQL Karşılığı: SELECT COUNT(*) FROM Siparisler
        var siparisSayisi = await _context.Siparisler.CountAsync();

        // 3. TOPLAM ÜRÜN (Count)
        var urunSayisi = await _context.Urunler.CountAsync();

        // 4. EN PAHALI ÜRÜNÜN ADI (Ordering + First)
        // SQL: SELECT TOP 1 Ad FROM Urunler ORDER BY Fiyat DESC
        var enPahaliUrun = await _context.Urunler
                                         .OrderByDescending(x => x.Fiyat) // Fiyata göre çoktan aza sırala
                                         .Select(x => x.Ad)               // Sadece 'Ad' sütununu seç (Hepsini çekme)
                                         .FirstOrDefaultAsync();          // İlkini al (Yoksa null dön)

        // DTO'yu doldurup gönderiyoruz
        return new DashboardRaporDTO(
            ToplamCiro: ciro,
            ToplamSiparisSayisi: siparisSayisi,
            ToplamUrunSayisi: urunSayisi,
            EnPahaliUrunAdi: enPahaliUrun ?? "Ürün Yok" // Eğer null gelirse (hiç ürün yoksa)
        );
    }
}