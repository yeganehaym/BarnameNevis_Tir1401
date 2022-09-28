using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BarnameNevis1401.ApplicationService;
using BarnameNevis1401.Core;
using BarnameNevis1401.Data;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Users;
using BarnameNevis1401.Email;
using BarnameNevis1401.Infrastructure;
using BarnameNevis1401.Models;
using BarnameNevis1401.SmsManagers;
using DNTCaptcha.Core;
using Ghasedak.Core;
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MimeMessage = MimeKit.MimeMessage;

namespace BarnameNevis1401.Controllers;

public class AuthController : Controller
{
   private ApplicationDbContext _context;
   private readonly IWebHostEnvironment _env;
   private IUserService _userService;
   private EmailSettings _emailSettings;

   public AuthController(ApplicationDbContext context,IWebHostEnvironment _env, IUserService userService,IOptionsSnapshot<EmailSettings> options)
   {
      _context = context;
      this._env = _env;
      _userService = userService;
      _emailSettings = options.Value;
   }

   public IActionResult Login(string returnUrl)
   {
      ViewData["ReturnUrl"] = returnUrl;
      ViewBag.ReturnUrl = returnUrl;
      return View();
   }

   [HttpPost]
   [ValidateDNTCaptcha(ErrorMessage = "لطفا کد امنیتی را وارد کنید",
      CaptchaGeneratorLanguage = Language.Persian,
      CaptchaGeneratorDisplayMode = DisplayMode.ShowDigits)]
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
         claim.AddClaim(new Claim(ClaimTypes.SerialNumber,user.SerialNumber));
         if(user.IsAdmin)
            claim.AddClaim(new Claim(ClaimTypes.Role,"Admin"));
         
         var claimPrincipals = new ClaimsPrincipal(claim);
         await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,claimPrincipals);
         if (String.IsNullOrEmpty(model.ReturnUrl) == false && model.ReturnUrl!="/")
            return Redirect(model.ReturnUrl);
         return RedirectToAction("Index", "Home");
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
         if (isExists != null)
         {
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
         newUser.GenerateSerialNumber();
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

   public IActionResult CheckEmailSettings()
   {
      return Content($"Server={_emailSettings.SmtpServer} - Username={_emailSettings.Username} -" +
                     $"Password={_emailSettings.Password} - Port={_emailSettings.Port}");
   }
   private bool RemoteServerCertificateValidationCallback(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
   {
      //Console.WriteLine(certificate);
      return true;
   }

   public async Task<IActionResult> ChangeSerialNumber()
   {
      var userId = User.GetUserId();
      var user = await _userService.FindUserAsync(userId);
      user.GenerateSerialNumber();
      var rows = await _context.SaveChangesAsync();
      return Content("Rows=" + rows);
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
      
            //============= Message ==================
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("آموزشگاه برنامه نویس",_emailSettings.Username));
            message.To.Add(new MailboxAddress(otp.User.LastName,otp.User.Email));
            message.Subject = "به مدیریت تصاویر خوش آمدید";
            
            var body = new TextPart(TextFormat.Html)
            {
               Text = "به برنامه <a href='https://barnameneviss.ir/'>برنامه نویس</a> خوش آمدید"
            };

            var multipartBody = new Multipart("mixed");
            multipartBody.Add(body);
            
            var stream = new FileStream(Path.Combine(_env.WebRootPath,"1.jpg"), FileMode.Open);
            var attachment = new MimePart ("image/jpg") {
               Content = new MimeContent (stream),
               ContentDisposition = new ContentDisposition (ContentDisposition.Attachment),
               ContentTransferEncoding = ContentEncoding.Base64,
               FileName = Path.GetFileName ("berry.jpg")
            };
            multipartBody.Add (attachment);
            message.Body = multipartBody;
            
            //===============================
            
            //send email
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteServerCertificateValidationCallback);
            var client = new SmtpClient();
            client.Connect(_emailSettings.SmtpServer,_emailSettings.Port,_emailSettings.SSL);
            client.Authenticate(_emailSettings.Username,_emailSettings.Password);
            client.Send(message);
            client.Disconnect(true);
            
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