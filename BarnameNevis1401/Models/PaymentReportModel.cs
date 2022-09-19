namespace BarnameNevis1401.Models;

public class PaymentReportModel
{
    public int Id { get; set; }
    public long Price { get; set; }
    public int Vat { get; set; }
    public long Discount { get; set; }
    public int Status { get; set; }
    public string PaymentDate { get; set; }
}