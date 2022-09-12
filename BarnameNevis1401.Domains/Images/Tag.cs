using BarnameNevis1401.Domains.Users;

namespace BarnameNevis1401.Domains.Images;

public class Tag:BaseEntity
{
    public string Name { get; set; }
    
    public List<ImageTag> ImageTags { get; set; }

    public User User { get; set; }
    public int? UserId { get; set; }
    
}