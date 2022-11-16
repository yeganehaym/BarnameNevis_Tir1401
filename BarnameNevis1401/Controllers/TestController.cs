using System.Text;
using BarnameNevis1401.Filters;
using BarnameNevis1401.Models;
using BarnameNevis1401.Patterns.PoolPattern;
using BarnameNevis1401.Patterns.SingltonPattern;
using BarnameNevis1401.SmsManagers;
using ElmahCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using RestSharp;

namespace BarnameNevis1401.Controllers;

[AllowAnonymous]
public class TestController : Controller
{
    private ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    // GET
    [MyAuthorize(Roles = "admin")]

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
        var smsManager=SmsFactory.Get(id);
        var smsCode = smsManager.SendMessage("0912...", "my sms");
        var rep = smsManager.GetRepository();

        return Content($"SmsCode: {smsCode} - Rep: {rep}");
    }

    [HttpGet]
    public IActionResult Occasions()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Occasions(DateViewModel model)
    {
        var url = $"https://persiancalapi.ir/jalali/{model.Year}/{model.Month}/{model.Day}";
        var client = new RestClient();
        var request = new RestRequest(url,Method.Get);
        var response=await client.ExecuteAsync<Occasion>(request);
     /*  if (response.IsSuccessful)
        {
            var json = response.Content;
            var occasion=JsonConvert.DeserializeObject<Occasion>(json);
            model.Occasion = occasion;
            return View(model);
        }*/

     if (response.IsSuccessful)
     {
         model.Occasion = response.Data;
         return View(model);
     }

        return Content("Failed");
    }
    
    public async Task<IActionResult> TestPost()
    {
        var url = $"https://persiancalapi.ir/jalali";
        var client = new RestClient();
        var request = new RestRequest(url,Method.Post);
        request.AddHeader("authorization", "Basic 3433434");
        
        var response=await client.ExecuteAsync<Occasion>(request);
        /*  if (response.IsSuccessful)
           {
               var json = response.Content;
               var occasion=JsonConvert.DeserializeObject<Occasion>(json);
               model.Occasion = occasion;
               return View(model);
           }*/

        

        return Content("Failed");
    }

    public async Task SendSms(string mobile,string code)
    {
        var smsManager = SmsFactory.Get();
        await smsManager.SendSmsTemplate("Login1401", mobile, new[] { code });
    }
    public async Task<IActionResult> SendSms2()
    {
        var x = DateTime.Now.AddDays(1) - DateTime.Now.AddMinutes(-30);

        BackgroundJob.Schedule(() => SendSms("09365437062", "0000"), TimeSpan.FromMinutes(1));
        return Content("OK");
    }
    public async Task<IActionResult> SendSms3()
    {
        RecurringJob.AddOrUpdate(
            "myrecurringjob",
            () => Console.WriteLine("Recurring!"),
            "* * * * *");
        return Content("OK");
    }

    public IActionResult TestElmah()
    {
        _logger.Log(LogLevel.Information,"تقسیم بر صفر");
        int c = 0;
        try
        {
            var a = 10;
            var b = a - 10;
            c = a / b;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            HttpContext.RaiseError(e);
        }

        return Content(c.ToString());
    }

    public IActionResult Pool()
    {

        string s = "";
        for (int i = 0; i < 10; i++)
        {
            var ground = PoolSource.GetGround();
            s += i + ": " + ground.CreationDate.ToString("HH:mm:ss") + " - " + ground.IsClosed + "|";
            var t = new Thread(new ThreadStart(()=>{ground.Connect();}));
            t.Start();
            Thread.Sleep(3000);
        }

        return Content(s);

    }

    public ActionResult TestSingleton()
    {
        var obj = MySingletonCreator.CreateInstance();
        return new EmptyResult();
    }

    public IActionResult TestPolicy()
    {
       var p= Policy
            .TimeoutAsync(TimeSpan.FromSeconds(5));
       p.ExecuteAsync( (ct) =>
       {
           /*var s = "";
           for (int i = 0; i < 1000000000; i++)
           {
               s += i.ToString();
               if (ct.IsCancellationRequested)
                   return Task.CompletedTask;
           }*/

           var s = new StringBuilder();
           for (int i = 0; i < 1000000000; i++)
           {
               s.Append(i);
               if (ct.IsCancellationRequested)
                   return Task.CompletedTask;
           }

           return Task.CompletedTask;
       },CancellationToken.None);

       return Content("OK");
    }
}