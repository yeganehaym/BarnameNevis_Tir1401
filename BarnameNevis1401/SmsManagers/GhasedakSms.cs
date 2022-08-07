using Ghasedak.Core;

namespace BarnameNevis1401.SmsManagers;

public class GhasedakSms:ISmsManager
{
    public string SendMessage(string mobileNo, string message)
    {
        throw new NotImplementedException();
    }

    public int GetRepository()
    {
        throw new NotImplementedException();
    }

    public async Task<string> SendSmsTemplate(string template,string mobileNo, string[] param)
    {
        Ghasedak.Core.Api api = new Api("0808138e234ab8ddc7f5ee48437105c39d940d6b5137b6f2a33d6dcb2f2f8e3c");
        var result = await api.VerifyAsync(1, template, new[] { mobileNo },
            param[0],
            param.Length<2?null:param[1],
            param.Length<3?null:param[2],
            param.Length<4?null:param[3],
            param.Length<5?null:param[4],
            param.Length<6?null:param[5],
            param.Length<7?null:param[6],
            param.Length<8?null:param[7],
            param.Length<9?null:param[8],
            param.Length<10?null:param[9]);

        return result.Items[0].ToString();
    }
}