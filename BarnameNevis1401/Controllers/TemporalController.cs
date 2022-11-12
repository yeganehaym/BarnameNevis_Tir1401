using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.Controllers;

public class TemporalController : Controller
{
    private IUnitOfWork _uow;
    // GET
    public TemporalController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public IActionResult Index()
    {
        _uow
            .Set<ShopItem>()
            .Add(new ShopItem()
            {
                Name = "LG",
                Price = 25000,
                Description = "Best TV"
            });
        var rows=_uow.SaveChanges();

        return Content("Rows=" + rows);
    }
    public IActionResult Index2()
    {
        var item = _uow
            .Set<ShopItem>()
            .Find(1);

        item.Price += 5000;
        var rows=_uow.SaveChanges();

        return Content("Rows=" + rows);
    }
    
    public IActionResult Records()
    {
        var items = _uow
            .Set<ShopItem>()
            .ToList();


        return Json(items);
    }
    
    public IActionResult Records2()
    {
       
        var items = _uow
            .Set<ShopItem>()
            //.TemporalBetween(item.CreationDate,DateTime.Now)
            .TemporalAll()
            .Select(x=> new
            {
                Name=x.Name,
                Price=x.Price,
                Start=EF.Property<DateTime>(x,"PeriodStart"),
                End=EF.Property<DateTime>(x,"PeriodEnd"),
            })
            .OrderBy(x=>x.Start)
            .ToList();

        var rnd = new Random();
        var value = rnd.Next(1, 10);

        return Json(items);
    }
}