using ETicaretAPI.Models;
using ETicaretAPI.Models.DTOs;

namespace ETicaretAPI.Services;

public interface ISiparisService
{
    // Asenkron (Task) dönüş tipleri
    Task<List<Siparis>> TumunuGetirAsync();
    Task SiparisOlusturAsync(SiparisOlusturDTO dto);
}