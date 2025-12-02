namespace ETicaretAPI.Models.DTOs;

public record UrunEkleDTO(
    string Ad,
    string Aciklama,
    decimal Fiyat,
    int StokAdedi,
    int KategoriId
);