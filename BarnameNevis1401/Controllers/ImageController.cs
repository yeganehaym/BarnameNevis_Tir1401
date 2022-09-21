using System.Security.Claims;
using BarnameNevis1401.Core;
using BarnameNevis1401.Data;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Images;
using BarnameNevis1401.Infrastructure;
using BarnameNevis1401.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarnameNevis1401.Controllers;

[Authorize]
public class ImageController : Controller
{
    private IWebHostEnvironment _env;
    private ApplicationDbContext _context;
    private IImageService _imageService;
    public ImageController(IWebHostEnvironment env, ApplicationDbContext context, IImageService imageService)
    {
        _env = env;
        _context = context;
        _imageService = imageService;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(ImageUploadModel model)
    {
        
        if (ModelState.IsValid)
        {
            var isValid = new Utils().IsValidImage(model.Picture.FileName);
            if (!isValid)
            {
                ModelState.AddModelError("Picture","فرمت تصویر معتبر نمیباشد");
                return View(model);
            }
            var path = Path.Combine(_env.ContentRootPath, "Pictures");
            var imageName = new Utils().SaveFile(model.Picture, path);

            var userId = HttpContext.User.GetUserId();

            byte[] image = null;
            using (var ms=new MemoryStream())
            {
                model.Picture.CopyTo(ms);
                image = ms.ToArray();
            }
            var imageItem = new ImageItem()
            {
                FileName = imageName,
                FileSize = model.Picture.Length,
                Type = model.Picture.ContentType,
                UserId = userId,
                Image = image,
                Names = new []{"canon","fp2","light"},
                UploadTimeDuration = TimeSpan.FromSeconds(5)
            };
            _context.ImageItems.Add(imageItem);
            var rows = _context.SaveChanges();
            if (rows == 0)
            {
                ModelState.AddModelError("Picture","خطا در ذخیره تصویر");
                return View(model);
            }
            return RedirectToAction("Index");

        }

        return View(model);
    }

    public IActionResult ShowImage(int id)
    {
        var imageItem = _context.ImageItems.FirstOrDefault(x => x.Id == id);
        if (imageItem == null)
            return NotFound();

        var path = Path.Combine(_env.ContentRootPath, "Pictures", imageItem.FileName);
        return File(new FileStream(path, FileMode.Open), imageItem.Type);
    }

    public IActionResult List(int id=1)
    {
        var pageSize = 12;
        var skip = (id - 1) * pageSize;
        
        var images = _context.ImageItems
            .OrderByDescending(x=>x.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToList();
        
        return View(images);
    }

    [HttpGet]
    public async Task<IActionResult> SetTags(int id)
    {
        var image = await _imageService.GetImage(id);
        if (image == null)
            return NotFound();
        
        return View(image);
    }
    
}