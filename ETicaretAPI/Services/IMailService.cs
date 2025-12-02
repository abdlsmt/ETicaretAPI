namespace ETicaretAPI.Services;

public interface IMailService
{
    Task SiparisMailiGonderAsync(string email, int siparisId);
}