namespace BarnameNevis1401.Data.Entities;

public class Tag:BaseEntity
{
    public string Name { get; set; }
    
    public List<ImageTag> ImageTags { get; set; }

}