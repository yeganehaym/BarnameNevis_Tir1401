using System.ComponentModel.DataAnnotations;

namespace BarnameNevis1401.Models;

public class LoginModel
{
    [Required]
    [MaxLength(20)]
    [MinLength(4)]
    public string Username { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    public string ReturnUrl { get; set; }
}