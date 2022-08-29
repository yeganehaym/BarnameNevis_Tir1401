using BarnameNevis1401.Domains.Users;

namespace BarnameNevis1401.Domains.Images;

public class ImageItem:BaseEntity
{
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string Type { get; set; }
    
    public User User { get; set; }
    public int UserId { get; set; }
    public List<ImageTag> ImageTags { get; set; }
    
    public byte[] Image { get; set; }
    
    public TimeSpan UploadTimeDuration { get; set; }
    public string[] Names { get; set; }
}