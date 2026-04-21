using System.Net;
using System.Net.Mail;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels.Contact;
using Microsoft.Extensions.Configuration;

namespace LerningApp.Services.Data;

public class ContactEmailSender(IConfiguration configuration) : IContactEmailSender
{
    public async Task SendContactMessageAsync(ContactFormViewModel model)
    {
        var host = configuration["EmailSettings:SmtpHost"];
        var port = int.Parse(configuration["EmailSettings:SmtpPort"]!);
        var senderEmail = configuration["EmailSettings:SenderEmail"];
        var receiverEmail = configuration["EmailSettings:ReceiverEmail"];
        var password = configuration["EmailSettings:SenderPassword"];
        
        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(senderEmail) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(receiverEmail))
        {
            throw new InvalidOperationException("Email settings are not configured correctly.");
        }
        
        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(senderEmail, password)
        };
        var body = $@"
                    <h2>New contact message</h2>
                    <p><strong>Name:</strong> {model.Name}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Message:</strong></p>
                    <p>{model.Message}</p>
                ";

        using var message = new MailMessage(
            from: senderEmail,
            to: receiverEmail,
            subject: model.Subject,
            body: body)
        {
            IsBodyHtml = true
        };

        message.ReplyToList.Add(new MailAddress(model.Email, model.Name));
        
        await client.SendMailAsync(message);
    }
}