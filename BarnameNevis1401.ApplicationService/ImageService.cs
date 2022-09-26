using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Images;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.ApplicationService;

public class ImageService:IImageService
{
    private ApplicationDbContext _context;

    public ImageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> GetSumOfBytes(int userId)
    {
        return await _context
            .ImageItems
            .Where(x => x.UserId == userId)
            .SumAsync(x => x.FileSize);
    }

    public async Task<ImageItem> GetImage(int id)
    {
        return await _context
            .ImageItems
            .FindAsync(id);
    }

    public async Task<bool> IsTagRegisteredAsync(int imageId,int tagId)
    {
        return await _context
            .ImageTags
            .AnyAsync(x => x.ImageItemId == imageId && x.TagId == tagId);
    }

    public async Task NewImageTagAsync(ImageTag imageTag)
    {
        await _context.ImageTags.AddAsync(imageTag);
    }

    public async Task<List<Tag>> GetTagsAsync(int id)
    {
        return await _context
            .ImageTags
            .Where(x => x.ImageItemId == id)
            .Select(x => x.Tag)
            .ToListAsync();
    }
}