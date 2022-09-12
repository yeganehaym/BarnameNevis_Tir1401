using System.ComponentModel.DataAnnotations;
using BarnameNevis1401.Domains.Users;

namespace BarnameNevis1401.Domains.Payments;

public class Payment:BaseEntity
{
    public long Price { get; set; }
    public PaymentStatus Status { get; set; }
    public Gateway Gateway { get; set; }
    public DateTime? PaymentTime { get; set; }
    public string RefCode { get; set; }
    public int Discount { get; set; }
    public int VAT { get; set; }
    public long FinalPrice => (Price - Discount)+(Price - Discount) * VAT / 100; 
    
    public long Size { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
}

public enum Gateway
{
    Zarinpal
}
public enum PaymentStatus
{
    [Display(Name = "نامشخص")]
    None,
    [Display(Name = "پرداخت موفق")]
    Success,
    [Display(Name = "پراخت ناموفق")]
    Failed
}

public static class EnumUtils
{
    public static string GetName(this Enum en)
    {
        var type = en.GetType().GetMember(en.ToString()).First();
        
        foreach (var attr in type.GetCustomAttributes(true))
        {
            var attribute = attr as DisplayAttribute;

            if (attribute != null)
            {
                return attribute.Name;
            }
            
        }

        return en.ToString();
    }
}