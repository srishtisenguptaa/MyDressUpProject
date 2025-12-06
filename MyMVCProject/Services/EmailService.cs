using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOtpAsync(string email, string otp)
    {
        var smtpHost = _config["Smtp:Host"];
        var smtpPort = int.Parse(_config["Smtp:Port"]);
        var smtpUser = _config["Smtp:Username"];
        var smtpPass = _config["Smtp:Password"];

        var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(smtpUser, "DressUp"),
            Subject = "OTP for Login",
            Body = $"Your OTP is: {otp}",
            IsBodyHtml = true
        };

        message.To.Add(email);

        await client.SendMailAsync(message);
    }

    public async Task SendResetLinkAsync(string toEmail, string link)
    {
        var mail = new MailMessage();
        mail.To.Add(toEmail);
        mail.Subject = "Reset Your Password";
        mail.Body = $"Click here to reset your password: {link}";
        mail.From = new MailAddress("srishtisengupta544@gmail.com");

        using var smtp = new SmtpClient("smtp.gmail.com", 587);
        smtp.Credentials = new NetworkCredential("srishtisengupta544@gmail.com", "sdhe ncco itcc kdno");
        smtp.EnableSsl = true;
        await smtp.SendMailAsync(mail);
    }
}
