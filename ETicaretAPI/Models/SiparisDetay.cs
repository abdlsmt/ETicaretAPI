using System.Text.Json.Serialization;

namespace ETicaretAPI.Models
{
    public class SiparisDetay
    {
        public int Id { get; set; }

        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; } // O anki fiyatı saklarız (Tarihçeli data)

        // --- İLİŞKİ 1: Hangi Sipariş? ---
        public int SiparisId { get; set; }
        [JsonIgnore] // Sonsuz döngüyü önlemek için
        public Siparis? Siparis { get; set; }

        // --- İLİŞKİ 2: Hangi Ürün? ---
        public int UrunId { get; set; }
        public Urun? Urun { get; set; }
    }
}