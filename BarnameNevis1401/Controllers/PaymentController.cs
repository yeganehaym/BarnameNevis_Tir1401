using System.Data;
using System.Globalization;
using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Payments;
using BarnameNevis1401.Models;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Parbad;
using Parbad.Gateway.ZarinPal;

namespace BarnameNevis1401.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class PaymentController : Controller
{
    private IPaymentService _paymentService;
    private IOnlinePayment _onlinePayment;
    private ApplicationDbContext _context;
    private IUserService _userService;
    private IWebHostEnvironment _env;

    private readonly IConfiguration _configuration;

    // GET
    public PaymentController(IPaymentService paymentService,IConfiguration _configuration, IOnlinePayment onlinePayment, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IUserService userService, IWebHostEnvironment env)
    {
        _paymentService = paymentService;
        this._configuration = _configuration;
        _onlinePayment = onlinePayment;
        _context = context;
        _userService = userService;
        _env = env;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(PayModel model)
    {
        if (ModelState.IsValid)
        {
            var discountcode = "Yalda";
            var discountPrice = 0;
            var vat = 9;
            if (model.DiscountCode.Trim().ToLower() == discountcode.ToLower())
            {
                discountPrice = 5000;
            }

            var cost = int.Parse(_configuration["PricePerGB"]);
            var price = cost * model.Size;
            var payment = new Payment()
            {
                VAT = vat,
                Discount = discountPrice,
                Gateway = Gateway.Zarinpal,
                Price = price,
                RefCode = "",
                UserId = User.GetUserId(),
                Size = model.Size*1024*1024*1204
            };
            await _paymentService.AddNewPaymentAsync(payment);

            var rows = await _context.SaveChangesAsync();

            if (rows == 0)
            {
                ModelState.AddModelError("Global","عدم ذخیره اطلاعات");
                return View(model);
            }

            var callbackUrl = Url.Action("Callback", "Payment", null, Request.Scheme, Request.Host.Value);
            var response=await _onlinePayment.RequestAsync(invoice =>
            {
                invoice.SetAmount(payment.FinalPrice)
                    .UseZarinPal()
                    //.SetGateway("Default")
                    .SetCallbackUrl(callbackUrl)
                    .SetTrackingNumber(payment.Id)
                    .SetZarinPalData("پرداخت حق فضا دیجیتال");
            });

            if (response.Status == PaymentRequestResultStatus.Succeed)
            {
                var url = response.GatewayTransporter.Descriptor.Url;
                //return Redirect(url);
                await response.GatewayTransporter.TransportAsync();
            }

            return View(model);
        }

        return View(model);
    }

    public async Task<IActionResult> Callback()
    {
        var invoice =await _onlinePayment.FetchAsync();
        var payment = await _paymentService.GetPaymentAsync((int)invoice.TrackingNumber);
        if (payment == null)
        {
            return Content("No Payment");
        }
        
        if (invoice.IsSucceed)
        {
            var verifyResult=await _onlinePayment.VerifyAsync(invoice);
            if (verifyResult.IsSucceed)
            {
                payment.Status = PaymentStatus.Success;
                payment.RefCode = verifyResult.TransactionCode;

                var user = await _userService.FindUserAsync(payment.UserId);
                user.Space += payment.Size;
                await _context.SaveChangesAsync();
                return Content("Payment Is OK");
            }
        }

        payment.Status = PaymentStatus.Failed;
        await _context.SaveChangesAsync();
        return Content("Not Ok");
    }

    [Authorize(Roles = "Admin,Writer,Accountant")]
    public IActionResult ExportExcel()
    {
        return View();
    }
    
    [Authorize(Roles = "Admin,Writer,Accountant")]
    [HttpPost]
    public async Task<IActionResult> ExportExcel(string from,string to)
    {
        var enFrom = from.ToGregorianDateTime();
        var enTo = to.ToGregorianDateTime();
        var payments = await _paymentService
            .GetPaymentsAsync(User.IsInRole("Admin")?null:User.GetUserId(), enFrom.Value, enTo.Value);

        var table = new System.Data.DataTable();
        var cap1 = new DataColumn("Date", typeof(string));
        cap1.Caption = "تاریخ";
        table.Columns.Add(cap1);
        table.Columns.Add(new DataColumn("Time"));
        table.Columns.Add(new DataColumn("Price"));
        table.Columns.Add(new DataColumn("Space"));
        table.Columns.Add(new DataColumn("RefCode"));
        table.Columns.Add(new DataColumn("Status"));
        table.Columns.Add(new DataColumn("Discount"));
        table.Columns.Add(new DataColumn("VAT"));

        foreach (var payment in payments)
        {
            var row = table.NewRow();
            row["Date"] = payment.PaymentTime.ToShortPersianDateString();
            row["Time"] = payment.PaymentTime.Value.ToString("HH:mm");
            row["Price"] = payment.FinalPrice.ToString("N0");
            row["Space"] = (payment.Size/1024/1024/1024) + "GB";
            row["RefCode"] = payment.RefCode;
            row["Status"] = payment.Status.GetName();
            row["Discount"] = payment.Discount.ToString("N0");
            row["VAT"] = payment.VAT;
            table.Rows.Add(row);
        }

        var path = Path.Combine(_env.ContentRootPath, "Excels");
        Directory.CreateDirectory(path);
        var objName = Guid.NewGuid()
            .ToString().Replace("-", "").Substring(0, 10) + ".xlsx";
        var fileName = Path.Combine(path,objName ) ;
        using (ExcelPackage package = new ExcelPackage())
        {
            var sheet=package.Workbook.Worksheets.Add("Payments");
            sheet.Cells["A1"].LoadFromDataTable(table, true, TableStyles.Medium3);
            sheet.Columns.AutoFit();
            sheet.View.RightToLeft = true;
            await package.SaveAsAsync(fileName,"123");
        }

        return PhysicalFile(fileName,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", objName);
    }
}