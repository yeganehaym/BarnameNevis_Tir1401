namespace BarnameNevis1401.Models;

public class SpaceReportModel
{
    public long TotalSpace { get; set; }
    public long UsedSpace { get; set; }

    public long Percent =>TotalSpace==0?0: UsedSpace * 100 / TotalSpace + DefaultPercent;
    public int DefaultPercent { get; set; }
}