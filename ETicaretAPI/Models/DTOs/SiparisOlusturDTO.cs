namespace ETicaretAPI.Models.DTOs;

// Record kullanarak tanımlama (Daha hafif ve güvenli)
public record SepetKalemiDTO(int UrunId, int Adet);

public record SiparisOlusturDTO(List<SepetKalemiDTO> Sepet);