using BarnameNevis1401.SmsManagers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarnameNevis1401.Controllers;

[Authorize]
public class TestController : Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    // GET
    public IActionResult TestCookie()
    {
        var cookie = Request.Cookies["MyCookie"];
        return View(model: cookie);
    }
    
   [AllowAnonymous]
    [HttpPost]
    public IActionResult TestCookie(string cookieValue)
    {
        Response.Cookies.Append("MyCookie",cookieValue,new CookieOptions() {
            MaxAge = TimeSpan.FromMinutes(1)
            
        });
        return View();
    }

    [AllowAnonymous]
    public IActionResult SendSms1(int id=1)
    {
        ISmsManager smsManager = null;
        switch (id)
        {
            case 1:
                smsManager= new TubaSms();
                break;
            default:
                smsManager = new Magfa();
                break;
        }
        var smsCode = smsManager.SendMessage("0912...", "my sms");
        var rep = smsManager.GetRepository();

        return Content($"SmsCode: {smsCode} - Rep: {rep}");
    }
}