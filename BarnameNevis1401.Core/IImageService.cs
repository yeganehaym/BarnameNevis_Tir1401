using BarnameNevis1401.Domains.Images;

namespace BarnameNevis1401.Core;

public interface IImageService
{
    Task<long> GetSumOfBytes(int userId);
    Task<ImageItem> GetImage(int id);
    Task<bool> IsTagRegisteredAsync(int imageId,int tagId);
    Task NewImageTagAsync(ImageTag imageTag);
    Task<List<Tag>> GetTagsAsync(int id);
}