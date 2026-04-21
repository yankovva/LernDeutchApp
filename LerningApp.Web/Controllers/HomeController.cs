using System.Diagnostics;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.ViewModels;
using LerningApp.Web.ViewModels.Contact;

using Microsoft.AspNetCore.Mvc;

using static LerningApp.Common.EntityErrorMessages.Common;

namespace LerningApp.Controllers;

public class HomeController(ILogger<HomeController> logger, IContactEmailSender emailSender) : Controller
{

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }
    [HttpGet]
    public IActionResult About()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await emailSender.SendContactMessageAsync(model);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = InvalidOperationMessage;
            throw;
        }
       
        TempData["SuccessMessage"] = "Successfully Sent Message.";
        return View(nameof(Contact));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}