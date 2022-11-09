using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Images;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.ApplicationService;

public class ImageService2:IImageService
{
    private IUnitOfWork _uow;

    public ImageService2(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<long> GetSumOfBytes(int userId)
    {
        return await _uow
            .Set<ImageItem>()
            .Where(x => x.UserId == userId)
            .SumAsync(x => x.FileSize);
    }

    public async Task<ImageItem> GetImage(int id)
    {
        return await  _uow
            .Set<ImageItem>()
            .FindAsync(id);
    }

    public async Task<bool> IsTagRegisteredAsync(int imageId,int tagId)
    {
        return await  _uow
            .Set<ImageTag>()
            .AnyAsync(x => x.ImageItemId == imageId && x.TagId == tagId);
    }

    public async Task NewImageTagAsync(ImageTag imageTag)
    {
        await  _uow
            .Set<ImageTag>().AddAsync(imageTag);
    }

    public async Task<List<Tag>> GetTagsAsync(int id)
    {
        return await _uow
            .Set<ImageTag>()
            .Where(x => x.ImageItemId == id)
            .Select(x => x.Tag)
            .ToListAsync();
    }
}