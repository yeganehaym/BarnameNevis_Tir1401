using System.Security.Claims;
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
      if (ModelState.IsValid)
      {
         var user = _context.Users.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);
         if (user == null)
         {
            ModelState.AddModelError("Username","چنین گاربری یافت نشد");
            return View(model);
         }

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