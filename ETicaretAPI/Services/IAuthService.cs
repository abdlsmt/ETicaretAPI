using ETicaretAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Services;

public interface IAuthService
{
    // Kayıt olma işlemi 
    Task<IdentityResult> RegisterAsync(RegisterDTO dto);

    // Giriş yapma işlemi 
    // bool yerine string? (Token dönecek, hata varsa null)
    Task<string?> LoginAsync(LoginDTO dto);
}