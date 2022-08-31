using BarnameNevis1401.Domains.Images;

namespace BarnameNevis1401.Core;

public interface ITagService
{
    Task<List<TagData>> GetTags(int skip, int take, string search = null);
    Task<int> GetTagsCount(string search = null);
    Task Remove(int id);
    Task<Tag> FindTagAsync(int id);
}