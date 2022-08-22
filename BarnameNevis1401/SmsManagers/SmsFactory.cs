namespace BarnameNevis1401.SmsManagers;
public static class SmsFactory{
    public static ISmsManager Get(int id=0)
    {                                                                   
        ISmsManager smsManager = null;
        switch (id)
        {
            case 0:
                smsManager = new GhasedakSms();
                break;
            case 1:
                smsManager= new TubaSms();
                break;
            default:
                smsManager = new Magfa();
                break;
        }

        return smsManager;
    }
}