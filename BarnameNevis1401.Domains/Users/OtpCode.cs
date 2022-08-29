namespace BarnameNevis1401.Domains.Users;

public class OtpCode:BaseEntity
{
    private const int Expire_Minutes = 10;

    public OtpCode()
    {
        ExpireTime = DateTime.Now.AddMinutes(Expire_Minutes);
    }
    public string Code { get; set; }
    public bool IsUsed { get; set; }
    public DateTime ExpireTime { get; set; }

    public bool IsValid => !IsUsed && ExpireTime > DateTime.Now;
    
    public User User { get; set; }
    public int UserId { get; set; }
}