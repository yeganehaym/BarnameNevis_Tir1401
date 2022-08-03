using System.Diagnostics;
using BarnameNevis1401.Data;
using BarnameNevis1401.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using BarnameNevis1401.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.Controllers;

[Authorize]
public class HomeController : Controller
{
    private ApplicationDbContext _applicationDbContext;

    public HomeController(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
}