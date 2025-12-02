using ETicaretAPI.Models;
using ETicaretAPI.Models.DTOs;

namespace ETicaretAPI.Services;

public interface IKategoriService
{
    // Dönüş tiplerini 'Task' ile sarmalıyoruz
    Task<List<Kategori>> TumunuGetirAsync();
    Task EkleAsync(KategoriEkleDTO dto);
}