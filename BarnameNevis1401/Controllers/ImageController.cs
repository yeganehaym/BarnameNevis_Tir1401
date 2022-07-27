using BarnameNevis1401.Models;
using Microsoft.AspNetCore.Mvc;

namespace BarnameNevis1401.Controllers;

public class ImageController : Controller
{
    private IWebHostEnvironment _env;

    public ImageController(IWebHostEnvironment env)
    {
        _env = env;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Upload(ImageUploadModel model)
    {
        if (ModelState.IsValid)
        {
            var path = Path.Combine(_env.ContentRootPath, "Pictures");
            var imageName = new Utils().SaveFile(model.Picture, path);
            return Content("Image Name=" + imageName);
        }

        return RedirectToAction("Index");
    }
    
    
}