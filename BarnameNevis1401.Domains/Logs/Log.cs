using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarnameNevis1401.Domains.Logs;

public class Log:BaseEntity
{
    public int? UserId { get; set; }
    public string Ip { get; set; }
    public DateTime RequestTime { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string Parameters { get; set; }
   // [Column(TypeName = "nvarchar(200)")]
    public string Agent { get; set; }
    
}