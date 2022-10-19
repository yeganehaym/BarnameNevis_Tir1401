using System.Diagnostics;
using BarnameNevis1401.Attributes;
using BarnameNevis1401.Data;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains;
using BarnameNevis1401.Domains.Users;
using Microsoft.AspNetCore.Mvc;
using BarnameNevis1401.Models;
using BarnameNevis1401.Resources;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace BarnameNevis1401.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class HomeController : Controller
{
    private ApplicationDbContext _applicationDbContext;
    private IStringLocalizer<HomeController> _localizer;
    private IStringLocalizer<SharedResource> _stringLocalizer;
    private IMemoryCache _memoryCache;

    public HomeController(ApplicationDbContext applicationDbContext, IStringLocalizer<HomeController> localizer
        , IStringLocalizer<SharedResource> stringLocalizer, IMemoryCache memoryCache)
    {
        _applicationDbContext = applicationDbContext;
        _localizer = localizer;
        _stringLocalizer = stringLocalizer;
        _memoryCache = memoryCache;
    }

    public IActionResult Index()
    {
        var value = _localizer["Hello"].Value;
        return View(model:value);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    [ResponseCache(CacheProfileName = "Profile1")]
    public IActionResult TestCache()
    {
        var time = DateTime.Now.ToString("HH:mm:ss");
        return Json(new
        {
            time
        });
    }
    
    [AllowAnonymous]
    [HttpGet]
    public IActionResult TestCache2()
    {
        var time = string.Empty;
        
        if (_memoryCache.Get<string>("time") != null)
        {
            time = _memoryCache.Get<string>("time");
        }
        else
        {
            time = DateTime.Now.ToString("HH:mm:ss");
            _memoryCache.Set("time", time, TimeSpan.FromSeconds(30));
        }
        return Json(new
        {
            time
        });
    }
    
    public IActionResult TestCache3()
    {
        return View("TestCache");
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ShowAttribute()
    {
        var name = UserStatus.Active.GetValue();
        
        return Content(name);
    }
    [HttpGet]
    public IActionResult New()
    {
        return View();
    }

    [HttpPost]
    public IActionResult New(Note note)
    {
        return Json(note);
    }

    public IActionResult TestInsert()
    {
        var user = new User()
        {
            FirstName = "ali",
            LastName = "yeganeh",
            CreationDate = DateTime.Now
        };

        _applicationDbContext.Users.Add(user);
        var rows = _applicationDbContext.SaveChanges();
        return Content("Rows=" + rows);
    }
    
    public IActionResult InsertProducts()
    {
        var p1 = new Product() { Name = "Laptop", Price = 10000, Discount = 10 };
        var p2 = new Product() { Name = "Tv LG", Price = 200, Discount = 0 };
        var p3 = new Product() { Name = "Tv Samsung", Price = 250, Discount = 5 };
        var p4 = new Product() { Name = "Mouse", Price = 500, Discount = 15 };

        _applicationDbContext.Products.AddRange(p1,p2,p3,p4);
        var rows = _applicationDbContext.SaveChanges();
        return Content("Rows=" + rows);
    }

    public IActionResult ListProducts(int? price)
    {
        var query = _applicationDbContext
            .Products
            .AsNoTracking()
            .AsQueryable();

        if (price.HasValue)
        {
            query = query
                .Where(x => x.Price < price);
        }

        var list = query
            .OrderBy(x => x.Name)
            .ToList();

        return View(list);
    }

    public IActionResult Remove(int id)
    {
        var product = _applicationDbContext.Products.Find(id);
        _applicationDbContext.Products.Remove(product);
        var rows = _applicationDbContext.SaveChanges();
        return RedirectToAction("ListProducts", "Home");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var product = _applicationDbContext.Products.FirstOrDefault(x => x.Id == id);
        return View(product);
    }
    
    [HttpPost]
    public IActionResult Edit(Product product)
    {
        var dbProduct = _applicationDbContext.Products.FirstOrDefault(x => x.Id == product.Id);
        dbProduct.Name = product.Name;
        dbProduct.Discount = product.Discount;
        dbProduct.Price = product.Price;
        dbProduct.ModificationDate=DateTime.Now;
        var rows = _applicationDbContext.SaveChanges();

        if (rows > 0)
            return RedirectToAction("ListProducts");
        
        return View(product);

    }

    public IActionResult ChangeLang(string id)
    {
        var cookieName = CookieRequestCultureProvider.DefaultCookieName;
        var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(id));
        Response.Cookies.Append(cookieName,cookieValue);
        return RedirectToAction("Index");
    }
}