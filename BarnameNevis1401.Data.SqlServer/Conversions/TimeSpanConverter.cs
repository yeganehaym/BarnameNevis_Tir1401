using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BarnameNevis1401.Data.SqlServer.Conversions;

public class TimeSpanConverter:ValueConverter<TimeSpan,long>
{
    public TimeSpanConverter()
        :base(v=>v.Ticks,v=>new TimeSpan(v))
    {
        
    }
}

public class StringArrayConverter : ValueConverter<string[], string>
{
    public StringArrayConverter():
        base(s=>string.Join(",",s),s=>s.Split(",",StringSplitOptions.RemoveEmptyEntries))
    {
        
    }
}