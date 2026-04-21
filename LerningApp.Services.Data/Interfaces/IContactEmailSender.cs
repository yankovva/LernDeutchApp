using LerningApp.Web.ViewModels.Contact;

namespace LerningApp.Services.Data.Interfaces;

public interface IContactEmailSender
{
    Task SendContactMessageAsync(ContactFormViewModel model);
}