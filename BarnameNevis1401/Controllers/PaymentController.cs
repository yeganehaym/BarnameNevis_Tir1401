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
    private IUserService _userService;

    private readonly IConfiguration _configuration;

    // GET
    public PaymentController(IPaymentService paymentService,IConfiguration _configuration, IOnlinePayment onlinePayment, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _paymentService = paymentService;
        this._configuration = _configuration;
        _onlinePayment = onlinePayment;
        _context = context;
        _userService = userService;
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
}