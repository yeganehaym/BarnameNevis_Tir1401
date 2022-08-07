namespace BarnameNevis1401.SmsManagers;

public interface ISmsManager
{
    string SendMessage(string mobileNo, string message);
    int GetRepository();
    Task<string> SendSmsTemplate(string template,string mobileNo, string[] param);
}