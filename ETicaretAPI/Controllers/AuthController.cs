using ETicaretAPI.Models.DTOs;
using ETicaretAPI.Services;
using Microsoft.AspNetCore.Mvc;


namespace ETicaretAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result.Succeeded)
        {
            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });
        }

        // Hata varsa (Örn: Şifre çok kısa, Email zaten var) listele
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var token = await _authService.LoginAsync(dto);

        if (token != null)
        {
            // Token döndürme
            return Ok(new { token = token });
        }

        return Unauthorized(new { message = "Email veya Şifre hatalı!" });
    }
}