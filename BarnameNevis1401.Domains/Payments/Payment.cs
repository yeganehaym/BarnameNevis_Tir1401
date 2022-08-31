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
    
    public User User { get; set; }
    public int UserId { get; set; }
}

public enum Gateway
{
    Zarinpal
}
public enum PaymentStatus
{
    None,
    Success,
    Failed
}