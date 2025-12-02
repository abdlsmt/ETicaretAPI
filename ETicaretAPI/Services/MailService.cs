using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace ETicaretAPI.Services;

public class MailService(IConfiguration _config) : IMailService
{
    public async Task SiparisMailiGonderAsync(string aliciEmail, int siparisId)
    {
        var email = new MimeMessage();

        // Kimden gidiyor?
        email.From.Add(new MailboxAddress(
            _config["MailSettings:SenderName"],
            _config["MailSettings:SenderEmail"]));

        // Kime gidiyor?
        email.To.Add(new MailboxAddress("Değerli Müşterimiz", aliciEmail));

        // Konu ne?
        email.Subject = $"Siparişiniz Alındı! - Sipariş No: #{siparisId}";

        // İçerik ne? (HTML Formatında)
        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = $@"
            <div style='font-family: Arial; border: 1px solid #ccc; padding: 20px;'>
                <h2 style='color: #27ae60;'>Siparişiniz Başarıyla Oluşturuldu! 🚀</h2>
                <p>Merhaba,</p>
                <p><strong>#{siparisId}</strong> numaralı siparişiniz sistemimize ulaşmıştır.</p>
                <p>En kısa sürede kargoya verilecektir.</p>
                <br>
                <p>Bizi tercih ettiğiniz için teşekkür ederiz.</p>
                <small>ETicaret API Ekibi</small>
            </div>";

        email.Body = bodyBuilder.ToMessageBody();

        // 👇 SMTP İLE GÖNDERME İŞLEMİ
        using var client = new SmtpClient();
        try
        {

            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            // Gmail Sunucusuna Bağlan
            // SecureSocketOptions.StartTls -> Gmail için en doğru ayardır.
            await client.ConnectAsync(
                _config["MailSettings:Server"],
                int.Parse(_config["MailSettings:Port"]!),
                MailKit.Security.SecureSocketOptions.StartTls
            );
            // Giriş Yap
            await client.AuthenticateAsync(_config["MailSettings:UserName"], _config["MailSettings:Password"]);

            // Gönder
            await client.SendAsync(email);

            // Bağlantıyı Kes
            await client.DisconnectAsync(true);

            Console.WriteLine($"✅ [MAIL BAŞARILI] {aliciEmail} adresine gönderildi.");
        }
        catch (Exception ex)
        {
            // Hata olursa Hangfire bunu görür ve tekrar dener!
            Console.WriteLine($"❌ [MAIL HATASI] {ex.Message}");
            throw;
        }
    }
}