using ETicaretAPI.Models;
using ETicaretAPI.Models.DTOs;

namespace ETicaretAPI.Services; // File-Scoped Namespace (Girinti yok)

public interface IUrunService
{
    // Async (Task) yapısı
    Task<List<Urun>> TumunuGetirAsync();
    Task EkleAsync(UrunEkleDTO dto);
}