namespace BarnameNevis1401.Core;

public class RegisterCheck
{
    public bool UsernameExists { get; set; }
    public bool EmailExists { get; set; }
    public bool MobileExists { get; set; }

    public bool IsExists => UsernameExists || EmailExists || MobileExists;
}