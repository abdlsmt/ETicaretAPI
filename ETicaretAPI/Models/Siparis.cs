namespace ETicaretAPI.Models
{
    public class Siparis
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Now;
        public decimal ToplamTutar { get; set; }

        // Bir siparişin içinde birden çok detay satırı olur.
        public List<SiparisDetay>? SiparisDetaylari { get; set; }
    }
}