namespace ETicaretAPI.Models
{
    public class Urun
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;

        // Para birimleri için her zaman 'decimal' kullanılır.
        public decimal Fiyat { get; set; }

        public int StokAdedi { get; set; }

        // --- İLİŞKİ (Foreign Key) ---
        // Bu ürün hangi kategoriye ait?
        public int KategoriId { get; set; }

        // Kod içinde "urun.Kategori.Ad" diyebilmek için:
        public Kategori? Kategori { get; set; }
    }
}