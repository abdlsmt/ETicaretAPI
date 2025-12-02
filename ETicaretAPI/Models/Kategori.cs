using System.Text.Json.Serialization;

namespace ETicaretAPI.Models
{
    public class Kategori
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;

        // --- İLİŞKİ ---
        // Bir kategorinin birden çok ürünü olabilir.
        // JsonIgnore: Kategori çekerken ürünleri, ürünleri çekerken kategoriyi getirip döngüye girmesin.
        [JsonIgnore]
        public List<Urun>? Urunler { get; set; }
    }
}