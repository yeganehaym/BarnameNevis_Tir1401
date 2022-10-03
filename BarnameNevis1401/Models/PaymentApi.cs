namespace BarnameNevis1401.Models;

public class PaymentApi
{
    public long Price { get; set; }
    public int Status { get; set; }
    public string Gateway { get; set; }
    public string PaymentTime { get; set; }
    public string RefCode { get; set; }
    public int Discount { get; set; }
    public int VAT { get; set; }
}