namespace BarnameNevis1401.SmsManagers;

public class Magfa : ISmsManager
{
    public string SendMessage(string mobileNo, string message)
    {
        return "456Magfa";
    }

    public int GetRepository()
    {
        return 5000000;
    }

    public Task<string> SendSmsTemplate(string template, string mobileNo, string[] param)
    {
        throw new NotImplementedException();
    }
}