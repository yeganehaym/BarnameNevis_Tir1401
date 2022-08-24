using BarnameNevis1401.Data;
using BarnameNevis1401.Data.Entities;
using BarnameNevis1401.Models;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.Services;

public class TagService
{
    private ApplicationDbContext _context;

    public TagService(ApplicationDbContext context)
    {
        _context = context;
    }

    private IQueryable<Tag> GetTagQuery(string search)
    {
        var query= _context
            .Tags
            .AsQueryable();

        if (String.IsNullOrEmpty(search) == false)
        {
            query = query
                .Where(x => x.Name.Contains(search));
        }

        return query;
    }
    public async Task<List<TagViewModel>> GetTags(int skip, int take, string search = null)
    {
        var query = GetTagQuery(search);
        return await query
            .Select(x => new TagViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                DateTime = x.CreationDate,
                ImagesCount = x.ImageTags.Count()
            })
            .OrderByDescending(x=>x.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
    
    public async Task<int> GetTagsCount(string search = null)
    {
        var query = GetTagQuery(search);
        return await   query
            .CountAsync();
    }

    public async Task Remove(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if(tag==null)
            return;
        
        _context.Tags.Remove(tag);
    }
}