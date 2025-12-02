using ETicaretAPI.Models;
using ETicaretAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Services;

public class AuthService(
    UserManager<AppUser> _userManager,
    SignInManager<AppUser> _signInManager,
    ITokenService _tokenService) : IAuthService  // TokenService eklendi
{
    public async Task<IdentityResult> RegisterAsync(RegisterDTO dto)
    {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Ad = dto.Ad,
            Soyad = dto.Soyad
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        // Kayıt başarılıysa Rol Ata
        if (result.Succeeded)
        {
            // Yeni gelen herkes "Customer" (Müşteri) olur.
            // Admin olmak veritabanından elle veya özel panelle yapılır.
            await _userManager.AddToRoleAsync(user, "Customer");
        }

        return result;
    }

    public async Task<string?> LoginAsync(LoginDTO dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        if (!result.Succeeded) return null;

        // Şifre doğruysa Token üret ve döndür. Roller devreye girdiği için await ile çağırıyoruz.
        return await _tokenService.TokenOlustur(user);
    }
}