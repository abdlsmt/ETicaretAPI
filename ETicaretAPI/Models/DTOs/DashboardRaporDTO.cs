namespace ETicaretAPI.Models.DTOs;

public record DashboardRaporDTO(
    decimal ToplamCiro,
    int ToplamSiparisSayisi,
    int ToplamUrunSayisi,
    string EnPahaliUrunAdi
);