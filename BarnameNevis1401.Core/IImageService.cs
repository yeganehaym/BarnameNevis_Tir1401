using BarnameNevis1401.Domains.Images;

namespace BarnameNevis1401.Core;

public interface IImageService
{
    Task<long> GetSumOfBytes(int userId);
    Task<ImageItem> GetImage(int id);
}