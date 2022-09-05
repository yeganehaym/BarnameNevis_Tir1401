namespace BarnameNevis1401.Core;

public interface IImageService
{
    Task<long> GetSumOfBytes(int userId);
}