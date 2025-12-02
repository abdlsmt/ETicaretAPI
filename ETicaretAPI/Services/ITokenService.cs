using ETicaretAPI.Models;

namespace ETicaretAPI.Services;

public interface ITokenService
{
    Task<string> TokenOlustur(AppUser user);
}