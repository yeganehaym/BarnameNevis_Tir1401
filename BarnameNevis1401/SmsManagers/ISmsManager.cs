namespace BarnameNevis1401.SmsManagers;

public interface ISmsManager
{
    string SendMessage(string mobileNo, string message);
    int GetRepository();
}