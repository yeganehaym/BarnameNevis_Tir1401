using BarnameNevis1401.Domains.Images;
using BarnameNevis1401.Domains.Payments;

namespace BarnameNevis1401.Domains.Users;

public class User:BaseEntity
{
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    
    public string Username { get; set; }
    
    public string Email { get; set; }
    public string Password { get; set; }
    public string Mobile { get; set; }
    public bool IsActive { get; set; }
    
    public bool IsAdmin { get; set; }
    
    public long Space { get; set; }
    
    public string SerialNumber { get; set; }

    public void GenerateSerialNumber()
    {
        SerialNumber = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
    }

    public List<Payment> Payments { get; set; }
    public List<ImageItem> ImageItems { get; set; }
    public List<Tag> Tags { get; set; }
    public List<OtpCode> OtpCodes { get; set; }
}