namespace BarnameNevis1401.Data.Selectmodels;

public class RegisterCheck
{
    public bool UsernameExists { get; set; }
    public bool EmailExists { get; set; }
    public bool MobileExists { get; set; }

    public bool IsExists => UsernameExists || EmailExists || MobileExists;
}