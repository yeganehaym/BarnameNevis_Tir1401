using System.ComponentModel.DataAnnotations;

namespace BarnameNevis1401.Models;

public class OtpModel
{
    [StringLength(4)]
    public string Code { get; set; }
}