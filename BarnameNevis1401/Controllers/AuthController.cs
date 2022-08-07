using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BarnameNevis1401.Data;
using BarnameNevis1401.Data.Entities;
using BarnameNevis1401.Models;
using BarnameNevis1401.Services;
using BarnameNevis1401.SmsManagers;
using Ghasedak.Core;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BarnameNevis1401.Controllers;

public class AuthController : Controller
{
   private ApplicationDbContext _context;
   private UserService _userService;

   public AuthController(ApplicationDbContext context, UserService userService)
   {
      _context = context;
      _userService = userService;
   }

   public IActionResult Login()
   {
      return View();
   }

   [HttpPost]
   public async Task<IActionResult> Login(LoginModel model)
   {
      
      if (ModelState.IsValid)
      {
         var user = _userService.Login(model.Username, model.Password);
         if (user == null)
         {
            ModelState.AddModelError("Username","چنین گاربری یافت نشد");
            return View(model);
         }

         var s = "12";
         var x = s.ToInt();
         

         var claim = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
         claim.AddClaim(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
         claim.AddClaim(new Claim(ClaimTypes.GivenName,user.Username));
         claim.AddClaim(new Claim(ClaimTypes.Name,user.FullName));
         var claimPrincipals = new ClaimsPrincipal(claim);
         await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,claimPrincipals);
         return RedirectToAction("TestCookie", "Test");
      }
      return View(model);
   }

   public async Task<IActionResult> Logout()
   {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      return RedirectToAction("Login");
   }

   [HttpGet]
   public IActionResult Register()
   {
      return View();
   }

   [HttpPost]
   public async Task<IActionResult> Register(RegisterModel model)
   {
      if (ModelState.IsValid)
      {
         var isExists =await _userService.IsExists(model.Username,model.Email,model.MobileNumber);
         if (isExists.UsernameExists)
         {
            ModelState.AddModelError("Username","نام کاربری تکراری است");
         }
         if (isExists.MobileExists)
         {
            ModelState.AddModelError("MobileNumber","شماره همراه  تکراری است");
         }
         if (isExists.EmailExists)
         {
            ModelState.AddModelError("Email","ایمیل  تکراری است");
         }

         if (isExists.IsExists)
         {
            return View(model);

         }

         var newUser = new User()
         {
            FirstName = "",
            LastName = "",
            
            Username = model.Username,
            Password = model.Password.Hash(),
            Email = model.Email,
            Mobile = model.MobileNumber
         };
         _userService.NewUser(newUser);
         var otpCode = new OtpCode()
         {
            Code = Utils.RandomString(4,RandomType.OnlyNumbers),
            User = newUser
         };
         _userService.AddOtpCode(otpCode);
         var rows = _context.SaveChanges();
         if (rows > 0)
         {
            //send sms
            BackgroundJob.Enqueue(() => SendSms(model.MobileNumber, otpCode.Code));
            return RedirectToAction("Otp");
         }
         else
         {
            return View(model);
         }
      }
      return View(model);
   }

   public async Task SendSms(string mobile,string code)
   {
      var smsManager = SmsFactory.Get();
      await smsManager.SendSmsTemplate("Login1401", mobile, new[] { code });
   }
   public IActionResult Otp()
   {
      return View();
   }
   
   [HttpPost]
   public IActionResult Otp(OtpModel model)
   {
      if (ModelState.IsValid)
      {
         var otp = _userService.GetOtpCode(model.Code);

         if (otp==null || !otp.IsValid)
         {
            ModelState.AddModelError("Code","کد مورد نظر معتبر نمیباشد");
            return View(model);
         }

         otp.IsUsed = true;
         otp.User.IsActive = true;

         var rows = _context.SaveChanges();
         if (rows > 0)
         {
            return RedirectToAction("Login");
         }
         else
         {
            ModelState.AddModelError("Code","خطا در ثبت اطلاعات");
            return View(model);
         }
      }
         
      return View();
   }
}