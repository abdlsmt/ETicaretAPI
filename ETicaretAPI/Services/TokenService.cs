using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ETicaretAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ETicaretAPI.Services;

// Primary Constructor ile UserManager'ı da alıyoruz
public class TokenService(IConfiguration _config, UserManager<AppUser> _userManager) : ITokenService
{
    // Metot artık async Task<string> dönüyor!
    public async Task<string> TokenOlustur(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Key"]!);

        // Kullanıcının rollerini çekiyoruz
        var roles = await _userManager.GetRolesAsync(user);

        // Claim Listesi (Kimlik Bilgileri)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        // Rolleri Token'a ekle (Önemli!)
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}