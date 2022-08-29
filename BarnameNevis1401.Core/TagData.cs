using DNTPersianUtils.Core;

namespace BarnameNevis1401.Core;

public class TagData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateTime { get; set; }
    public string Date => DateTime.ToShortPersianDateString();
    public string Time => DateTime.ToString("HH:mm");
    public int ImagesCount { get; set; }
}