using BarnameNevis1401.Domains.Images;

namespace BarnameNevis1401.Core;

public interface ITagService
{
    Task<List<TagData>> GetTags(int? userId,int skip, int take, string search = null);
    Task<int> GetTagsCount(int? userId,string search = null);
    Task Remove(int id);
    Task<Tag> FindTagAsync(int id);
    Task AddTagsAsync(List<Tag> newTags);
    Task<Tag?> FindTagAsync(string name);
    Task<int> AddTagAsync(Tag tag);
    Task<int> EditTagAsync(Tag tag);
    Task<List<Tag>> FilterTags(int getUserId, string query);
    Task<int> SearchTagAsync(string tag);
    Task<List<SPTAG>> FindTageByName(string name);
}