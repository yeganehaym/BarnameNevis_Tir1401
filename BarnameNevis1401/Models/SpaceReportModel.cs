namespace BarnameNevis1401.Models;

public class SpaceReportModel
{
    public long TotalSpace { get; set; }
    public long UsedSpace { get; set; }

    public long Percent => UsedSpace * 100 / TotalSpace + DefaultPercent;
    public int DefaultPercent { get; set; }
}