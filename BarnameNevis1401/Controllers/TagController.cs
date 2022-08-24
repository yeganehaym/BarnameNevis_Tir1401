using BarnameNevis1401.Data;
using BarnameNevis1401.Data.Entities;
using BarnameNevis1401.DataTable;
using BarnameNevis1401.Models;
using BarnameNevis1401.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarnameNevis1401.Controllers;

public class TagController : Controller
{
    private TagService _tagService;

    private ApplicationDbContext _context;
    // GET
    public TagController(TagService tagService, ApplicationDbContext context)
    {
        _tagService = tagService;
        _context = context;
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
        var result = new DataTableResult<TagViewModel>();
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
}