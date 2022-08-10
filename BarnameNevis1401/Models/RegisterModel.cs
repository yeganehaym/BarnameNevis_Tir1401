using System.ComponentModel.DataAnnotations;
using DNTPersianUtils.Core;

namespace BarnameNevis1401.Models;

public class RegisterModel
{
    [MinLength(6)]
    [MaxLength(20)]
    public string Username { get; set; }
    [MinLength(6)]
    [MaxLength(15)]
    public string Password { get; set; }
    [Compare("Password")]
    public string RePassword { get; set; }
    [EmailAddress()]
    public string Email { get; set; }
    [ValidIranianMobileNumber]
    public string MobileNumber { get; set; }
}