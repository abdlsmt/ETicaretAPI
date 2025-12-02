using ETicaretAPI.Models;
using ETicaretAPI.Models.DTOs;
using ETicaretAPI.Repositories;

namespace ETicaretAPI.Services;

// Primary Constructor
public class KategoriService(IGenericRepository<Kategori> _repository) : IKategoriService
{
    public async Task<List<Kategori>> TumunuGetirAsync()
    {
        // Repository'deki Async metodu çağırıyoruz
        return await _repository.TumunuGetirAsync();
    }

    public async Task EkleAsync(KategoriEkleDTO dto)
    {
        var yeniKategori = new Kategori { Ad = dto.Ad };

        // Ekleme işlemi de asenkron
        await _repository.EkleAsync(yeniKategori);
    }
}