using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace LerningApp.Services.Data;

using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender(IConfiguration configuration): IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var host = configuration["EmailSettings:SmtpHost"];
        var port = int.Parse(configuration["EmailSettings:SmtpPort"]!);
        var senderEmail = configuration["EmailSettings:SenderEmail"];
        var password = configuration["EmailSettings:SenderPassword"];
        
        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(senderEmail) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Email settings are not configured correctly.");
        }
        
        var client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(senderEmail, password)
        };

        using var message = new MailMessage(
            from: senderEmail,
            to: email,
            subject: subject,
            body: htmlMessage)
        {
            IsBodyHtml = true
        };
        
        await client.SendMailAsync(message);
    }
}