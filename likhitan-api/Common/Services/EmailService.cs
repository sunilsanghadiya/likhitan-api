using likhitan.Common.Services.Dtos;
using MailKit.Security;
using MimeKit;
namespace likhitan.Common.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendOtpAsync(string toEmail, string otp)
    {
        var emailSettings = _config.GetSection("EmailSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Your OTP Code ";

        message.Body = new TextPart("plain")
        {
            Text = $"Your OTP code is: {otp}. It will expire in {_config.GetValue<string>("OTP:Expiration")} minutes."
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();
        try
        {
            await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false; 
        }
    }

    public async Task<bool> SendPasswordChangeEmail(SendPasswordChangeEmailDto sendPasswordChangeEmailDto)
    {
        #region Validations
        if (string.IsNullOrWhiteSpace(sendPasswordChangeEmailDto.Email))
            return false;
        if (string.IsNullOrWhiteSpace(sendPasswordChangeEmailDto.ForwardedIp))
            return false;
        if (string.IsNullOrWhiteSpace(sendPasswordChangeEmailDto.RealIp))
            return false;
        if (string.IsNullOrWhiteSpace(sendPasswordChangeEmailDto.UserIp))
            return false;
        if (string.IsNullOrWhiteSpace(sendPasswordChangeEmailDto.UserAgent))
            return false;

        #endregion

        var emailSettings = _config.GetSection("EmailSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
        message.To.Add(new MailboxAddress("", sendPasswordChangeEmailDto.Email));
        message.Subject = "Your OTP Code ";

        message.Body = new TextPart("plain")
        {
            Text = $"Your password has been changed at:{DateTime.Now}, UserIP {sendPasswordChangeEmailDto.UserIp}, Browser {sendPasswordChangeEmailDto.UserAgent}"
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();
        try
        {
            await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
}
