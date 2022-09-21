using System.Drawing;
using BarnameNevis1401.Core;
using BarnameNevis1401.Models;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace BarnameNevis1401.Controllers;

public class ReportController : Controller
{
    private IPaymentService _paymentService;
    private IWebHostEnvironment _env;

    private IUserService _userService;
    // GET
    public ReportController(IPaymentService paymentService, IUserService userService, IWebHostEnvironment env)
    {
        _paymentService = paymentService;
        _userService = userService;
        _env = env;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> LoadReportData()
    {
        var payments = await _paymentService.GetPaymentsAsync(0, int.MaxValue, User.GetUserId(), null);
        var user = await _userService.FindUserAsync(User.GetUserId());

        /*var paymentList = new List<PaymentReportModel>();

        foreach (var payment in payments)
        {
            paymentList.Add(new PaymentReportModel()
            {
                Id = payment.Id,
                Discount = payment.Discount,
                Price = payment.Price,
                Status =(int) payment.Status,
                Vat = payment.VAT,
                PaymentDate = payment.PaymentTime.ToShortPersianDateString()
            });
        }*/

        var paymentList = payments
            .Select(payment => new PaymentReportModel()
            {
                Id = payment.Id,
                Discount = payment.Discount,
                Price = payment.Price,
                Status = (int)payment.Status,
                Vat = payment.VAT,
                PaymentDate = payment.PaymentTime.ToShortPersianDateString()
            })
            .ToList();
        
        
        var report = new StiReport();
        report.Load(StiNetCoreHelper.MapPath(this,"~/Reports/Report.mrt"));

        report.RegBusinessObject("User",new
        {
            Name=user.FirstName,
            user.LastName,
            user.Email,
            user.Mobile,
            user.Space
        });
        report.RegBusinessObject("Payments",paymentList);

        using (var fs=new FileStream(Path.Combine(_env.ContentRootPath,"Pictures","profile.png"),FileMode.Open))
        {
            report.Dictionary.Variables.Add("avatar",Image.FromStream(fs));
        }
        
        
        report.Dictionary.Variables.Add("PersianDate",DateTime.Now.ToShortPersianDateString());

        return StiNetCoreViewer.GetReportResult(this, report);
    }

    public IActionResult ViewerEvent()
    {
       return StiNetCoreViewer.ViewerEventResult(this);
    }
}