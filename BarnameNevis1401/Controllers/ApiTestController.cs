using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Images;
using BarnameNevis1401.Models;
using DNTPersianUtils.Core;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parbad.Storage.Abstractions.Models;
using Payment = BarnameNevis1401.Domains.Payments.Payment;

namespace BarnameNevis1401.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApiTestController:ControllerBase
{
    private ITagService _tagService;
    private ApplicationDbContext _context;
    private IPaymentService _paymentService;

    public ApiTestController(ITagService tagService, ApplicationDbContext context, IPaymentService paymentService)
    {
        _tagService = tagService;
        _context = context;
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<List<TagData>> GetTags(int skip,int take)
    {
        var tags = await _tagService.GetTags(null, skip, take);
        return tags;
    }

    [HttpPost]
    public async Task<IActionResult> NewTag([FromBody]TagsApiParams model)
    {
        var tag = new Tag()
        {
            Name = model.Name
        };
       var rows= await _tagService.AddTagAsync(tag);

        await _context.SaveChangesAsync();

        if (rows > 0)
            return Created("NewTag", new { id = tag.Id });
        return StatusCode(400);
        //return BadRequest();
    }

    /// <summary>
    /// ویرایش تگ
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(400,"Error in Register")]
    [Swashbuckle.AspNetCore.Annotations.SwaggerResponse(204,"tag is not found")]
    public async Task<IActionResult> UpdateTag([FromBody] UpdateTagByApi model)
    {
        var tag = await _tagService.FindTagAsync(model.Id);

        if (tag == null)
            return NoContent();
        tag.Name = model.Name;
        var rows = await _context.SaveChangesAsync();
        if (rows > 0)
            return Ok();
        return BadRequest();
    }


    public async Task<List<PaymentApi>> GetPayments()
    {
        var payments = await _paymentService.GetPaymentsAsync(0,int.MaxValue,null,null);

        TypeAdapterConfig<Payment, PaymentApi>
            .NewConfig()
            //.Ignore(dest => dest.Price)
            .Map(dest=>dest.Price,src=>src.FinalPrice)
            .Map(dest => dest.PaymentTime, src => src.PaymentTime.ToPersianDateTextify());
        
        var apiPayments = payments.Adapt<List<PaymentApi>>();
        return apiPayments;
    }
}