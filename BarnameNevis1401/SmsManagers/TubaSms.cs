namespace BarnameNevis1401.SmsManagers;

public class TubaSms : ISmsManager
{
    public string SendMessage(string mobileNo, string message)
    {
        return "123Tuba";
    }

    public int GetRepository()
    {
        return 50000;
    }
}