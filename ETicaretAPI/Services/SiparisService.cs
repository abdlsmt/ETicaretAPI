using ETicaretAPI.Data;
using ETicaretAPI.Models;
using ETicaretAPI.Models.DTOs;
using ETicaretAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using System.Security.Claims; // Mail göndermek için

namespace ETicaretAPI.Services;

// Primary Constructor: Gerekli tüm depoları ve Context'i buradan alıyoruz
public class SiparisService(
    IGenericRepository<Siparis> _siparisRepo,
    IGenericRepository<Urun> _urunRepo,
    ETicaretContext _context,
    IBackgroundJobClient _jobClient,
    IHttpContextAccessor _httpContextAccessor) : ISiparisService
{
    public async Task<List<Siparis>> TumunuGetirAsync()
    {
        // "Include" ile ilişkili verileri (Detaylar ve Ürün isimleri) getiriyoruz
        return await _siparisRepo.TumunuGetirAsync("SiparisDetaylari.Urun");
    }

    public async Task SiparisOlusturAsync(SiparisOlusturDTO dto)
    {
        // --- 1. KURAL: Sepet Çeşit Limiti ---
        if (dto.Sepet.Count > 10)
        { 
            throw new Exception("Bir siparişte en fazla 10 farklı ürün olabilir.");
        }

        // --- 2. KURAL: Ürün Başına Adet Limiti ---
        if (dto.Sepet.Any(x => x.Adet > 5))
        {
            throw new Exception("Her ürün için adet 1 veya daha fazla olmalıdır.");
        }

        // 🛡️ TRANSACTION BAŞLATIYORUZ
        // Veritabanı işlemleri (Stok düşme + Sipariş yazma) tek paket olur.
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var yeniSiparis = new Siparis
            {
                SiparisDetaylari = new List<SiparisDetay>(),
                Tarih = DateTime.Now
            };

            decimal toplamTutar = 0;

            foreach (var kalem in dto.Sepet)
            {
                // 1. Ürünü Bul
                var urun = await _urunRepo.IdIleGetirAsync(kalem.UrunId);
                if (urun == null) throw new Exception($"Ürün ID {kalem.UrunId} bulunamadı!");

                // 2. Stok Kontrolü
                if (urun.StokAdedi < kalem.Adet)
                    throw new Exception($"{urun.Ad} stoğu yetersiz! Kalan: {urun.StokAdedi}");

                // 3. Stoktan Düş
                urun.StokAdedi -= kalem.Adet;
                await _urunRepo.GuncelleAsync(urun);

                // 4. Detay Satırı Oluştur
                var detay = new SiparisDetay
                {
                    UrunId = urun.Id,
                    Adet = kalem.Adet,
                    BirimFiyat = urun.Fiyat // O anki fiyatı kilitliyoruz
                };
                yeniSiparis.SiparisDetaylari.Add(detay);
                toplamTutar += (urun.Fiyat * kalem.Adet);
            }

            if (toplamTutar < 100)
            {
                throw new Exception($"Minimum sipariş tutarı 100 TL olmalıdır. Sizin toplam tutarınız {toplamTutar} TL");
            }

            yeniSiparis.ToplamTutar = toplamTutar;

            // 5. Siparişi Kaydet
            await _siparisRepo.EkleAsync(yeniSiparis);

            // ✅ HER ŞEY BAŞARILIYSA ONAYLA
            await transaction.CommitAsync();

            // 👇 GİRİŞ YAPAN KULLANICININ MAİLİNİ BULUYORUZ
            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

            if (!string.IsNullOrEmpty(userEmail))
            {
                Console.WriteLine($"📧 [MAIL KUYRUĞA EKLENİYOR] Kime: {userEmail}");
                _jobClient.Enqueue<IMailService>(x => x.SiparisMailiGonderAsync(userEmail, yeniSiparis.Id));
            }
            else
            {
                // 👇 EĞER EMAIL YOKSA BURASI ÇALIŞACAK
                Console.WriteLine("⚠️ [UYARI] Kullanıcı kimliği bulunamadı! Mail gönderilmedi.");
            }
        }
        catch (Exception)
        {
            // ❌ HATA VARSA HER ŞEYİ GERİ AL (Stoklar eski haline döner)
            await transaction.RollbackAsync();
            throw; // Hatayı fırlat ki Controller veya Middleware yakalasın
        }
    }
}