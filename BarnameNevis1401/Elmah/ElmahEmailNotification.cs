using ElmahCore;

namespace BarnameNevis1401.Elmah;

public class ElmahEmailNotification:IErrorNotifier
{
    public void Notify(Error error)
    {
       //send sms,email,whatsapp,telegram...
       
       
    }

    public string Name { get; }
}