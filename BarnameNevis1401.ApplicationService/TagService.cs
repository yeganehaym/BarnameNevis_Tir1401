using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Images;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.ApplicationService;


public class TagService : ITagService
{
    private ApplicationDbContext _context;

    public TagService(ApplicationDbContext context)
    {
        _context = context;
    }

    private IQueryable<Tag> GetTagQuery(int? userId,string search)
    {
        var query = _context
            .Tags
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query
                .Where(x => x.UserId == userId.Value);
        }
        if (String.IsNullOrEmpty(search) == false)
        {
            query = query
                .Where(x => x.Name.Contains(search));
        }

        return query;
    }
    public async Task<List<TagData>> GetTags(int? userId,int skip, int take, string search = null)
    {
        var query = GetTagQuery(userId,search);
        return await query
            .Select(x => new TagData()
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
    
    public async Task<int> GetTagsCount(int? userId,string search = null)
    {
        var query = GetTagQuery(userId,search);
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

    public async Task<Tag> FindTagAsync(int id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task AddTagsAsync(List<Tag> newTags)
    {
        await _context.Tags.AddRangeAsync(newTags);
    }

    public async Task<Tag?> FindTagAsync(string name)
    {
        return await _context.Tags.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<int> AddTagAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> EditTagAsync(Tag tag)
    {
        var tagIsExited = await _context.Tags.FirstOrDefaultAsync(x => x.Name == tag.Name);
        if (tagIsExited == null)
        {
            var newTag = await _context.Tags.FindAsync(tag.Id);
            newTag.Name = tag.Name;
            newTag.ModificationDate = DateTime.Now;
            return await _context.SaveChangesAsync();
        }

        return 0;
    }

    public async Task<List<Tag>> FilterTags(int userId, string query)
    {
        return await _context
            .Tags
            .Where(x => x.UserId == userId && x.Name.StartsWith(query))
            .Skip(0)
            .Take(10)
            .ToListAsync();
    }

    public async Task<int> SearchTagAsync(string tag)
    {
        var dbTag = await _context
            .Tags
            .FirstOrDefaultAsync(x => x.Name == tag.Trim());
        if (dbTag == null)
            return 0;

        return dbTag.Id;
    }

    public async Task<List<SPTAG>> FindTageByName(string name)
    {
        //for insert,update,delete
        
        /*var x = _context
            .Database
            .ExecuteSqlRaw("update users set amount=amount+@amount",
                new [] {new SqlParameter("@amount",10000)});
                  _context
            .Database
            .ExecuteSqlInterpolated($"update users set amount=amount+{10000}");
            
                */

        //for Select
        return  await _context
            .SPTAGS
            .FromSqlRaw("exec USP_FIND_TAGS @name"
                , new[] { new SqlParameter("@name", name) })
            .ToListAsync();

    }
}

public class Test : ITest
{
    public Test(string s)
    {
        
    }
    public void X()
    {
        throw new NotImplementedException();
    }
}