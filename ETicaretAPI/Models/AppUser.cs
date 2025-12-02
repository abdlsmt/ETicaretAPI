using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Models;

// IdentityUser'dan miras alıyoruz.
// Böylece Email, Password, Username gibi alanlar otomatik geliyor.
public class AppUser : IdentityUser
{
    // Ekstra istediğimiz alanları buraya ekliyoruz
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
}