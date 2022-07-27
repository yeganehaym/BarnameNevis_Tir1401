using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarnameNevis1401.Data.Entities;

public class User:BaseEntity
{
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    
    public string Username { get; set; }
    
    public string Email { get; set; }
    public string Password { get; set; }
    public string Mobile { get; set; }
 
    
    public List<ImageItem> ImageItems { get; set; }
}