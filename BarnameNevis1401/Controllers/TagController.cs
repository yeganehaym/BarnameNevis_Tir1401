using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.DataTable;
using BarnameNevis1401.Domains.Images;
using BarnameNevis1401.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BarnameNevis1401.Controllers;

public class TagController : Controller
{
    private ITagService _tagService;
    private IWebHostEnvironment _env;

    private ApplicationDbContext _context;
    // GET
    public TagController(ITagService tagService, ApplicationDbContext context, IWebHostEnvironment env)
    {
        _tagService = tagService;
        _context = context;
        _env = env;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> LoadTags(DataTableParam param)
    {
        var s = Request.Query["search[value]"].ToString();
        var tags = await _tagService.GetTags(param.Start, param.Length,s);
        var total = await _tagService.GetTagsCount();
        var filter = await _tagService.GetTagsCount(s);
        var result = new DataTableResult<TagData>();
        result.Data = tags;
        result.RecordsTotal = total;
        result.RecordsFiltered = filter;
        result.Draw = param.draw;
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveTag(int id)
    {
        await _tagService.Remove(id);
        var rows = await _context.SaveChangesAsync();

        return Json(new { result = rows > 0 });
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveSoftTag(int id)
    {
        var tag =await _tagService.FindTagAsync(id);
        tag.IsRemoved = true;
        var rows = await _context.SaveChangesAsync();

        return Json(new { result = rows > 0 });
    }

    public IActionResult ReadTags()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ReadTags(IFormFile file)
    {
        var utils = new Utils();
        var path = Path.Combine(_env.ContentRootPath, "Excels");
        var fileName=utils.SaveFile(file,path);

        var fullPath = Path.Combine(path, fileName);
        using (ExcelPackage package=new ExcelPackage(new FileInfo(fullPath)))
        {
            var sheet = package.Workbook.Worksheets.First();
            var rowStart = sheet.Dimension.Start.Row;
            var rowEnd = sheet.Dimension.End.Row;

            var colStart =sheet.Dimension.Start.Column;
            var colEnd = sheet.Dimension.Start.Column;

            var tags = new List<string>();
            for (int i = rowStart;  i<= rowEnd; i++)
            {
                var tag = sheet.Cells[i, 1].Value.ToString();
                tags.Add(tag);
            }

            var newTags = tags
                .Select(x => new Tag()
                {
                    Name = x,
                    UserId = User.GetUserId()
                })
                .ToList();

            await _tagService.AddTagsAsync(newTags);
            var rows = await _context.SaveChangesAsync();
            if (rows > 0)
                return Content("OK");
            return Content("NOK");
        }
    }
}