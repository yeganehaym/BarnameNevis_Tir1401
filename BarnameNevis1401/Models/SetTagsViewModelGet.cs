using BarnameNevis1401.Domains.Images;

namespace BarnameNevis1401.Models;

public class SetTagsViewModelGet
{
    public ImageItem ImageItem { get; set; }
    public List<Tag> Tags { get; set; }
}