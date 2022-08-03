using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BarnameNevis1401.Data;
using BarnameNevis1401.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BarnameNevis1401.Controllers;

public class AuthController : Controller
{
   private ApplicationDbContext _context;

   public AuthController(ApplicationDbContext context)
   {
      _context = context;
   }

   public IActionResult Login()
   {
      return View();
   }

   [HttpPost]
   public IActionResult Login(LoginModel model)
   {
      
      var password = model.Password.Hash();
      if (ModelState.IsValid)
      {
         var user = _context.Users.FirstOrDefault(x => x.Username == model.Username && x.Password == password);
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
         HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,claimPrincipals);
         return RedirectToAction("TestCookie", "Test");
      }
      return View(model);
   }

   public IActionResult Logout()
   {
      HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      return RedirectToAction("Login");
   }
}