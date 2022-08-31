using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Payments;
using BarnameNevis1401.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parbad;
using Parbad.Gateway.ZarinPal;

namespace BarnameNevis1401.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private IPaymentService _paymentService;
    private IOnlinePayment _onlinePayment;
    private ApplicationDbContext _context;

    private readonly IConfiguration _configuration;

    // GET
    public PaymentController(IPaymentService paymentService,IConfiguration _configuration, IOnlinePayment onlinePayment, ApplicationDbContext context)
    {
        _paymentService = paymentService;
        this._configuration = _configuration;
        _onlinePayment = onlinePayment;
        _context = context;
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
                UserId = User.GetUserId()
            };
            await _paymentService.AddNewPaymentAsync(payment);

            var rows = await _context.SaveChangesAsync();

            if (rows == 0)
            {
                ModelState.AddModelError("Global","عدم ذخیره اطلاعات");
                return View(model);
            }

            var callbackUrl = Url.Action("Callback", "Payment", null, null, Request.Scheme);
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
                await response.GatewayTransporter.TransportAsync();
            }

            return View(model);
        }

        return View(model);
    }

    public async Task<IActionResult> Callback()
    {
        return new EmptyResult();
    }
}