using ETicaretAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Data;

public class ETicaretContext(DbContextOptions<ETicaretContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Kategori> Kategoriler { get; set; }
    public DbSet<Urun> Urunler { get; set; }
    public DbSet<Siparis> Siparisler { get; set; }
    public DbSet<SiparisDetay> SiparisDetaylari { get; set; }
}