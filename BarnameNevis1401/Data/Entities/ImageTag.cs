namespace BarnameNevis1401.Data.Entities;

public class ImageTag
{
    public ImageItem ImageItem { get; set; }
    public int ImageItemId { get; set; }
    public Tag Tag { get; set; }
    public int TagId { get; set; }
}