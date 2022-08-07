namespace BarnameNevis1401.SmsManagers;

public class TubaSms : ISmsManager
{
    public string SendMessage(string mobileNo, string message)
    {
        return "123Tuba";
    }

    public int GetRepository()
    {
        return 10000;
    }

    public Task<string> SendSmsTemplate(string template, string mobileNo, string[] param)
    {
        throw new NotImplementedException();
    }
}