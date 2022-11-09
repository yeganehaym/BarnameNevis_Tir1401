using BarnameNevis1401.CQRS.Commands.Users;
using BarnameNevis1401.CQRS.Query.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BarnameNevis1401.Controllers;

public class CQRSController : Controller
{
    private IMediator _mediator;

    public CQRSController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Index(CreatOrUpdateUserCommand command)
    {
        var user = await _mediator.Send(command);
        if (user == null)
        {
            ModelState.AddModelError("FirstName","Error");
        }
        return View(command);
    }

    public async Task<IActionResult> ReadUsers()
    {
        var users = await _mediator.Send(new GetUserListQuery()
        {
            Skip = 0,
            Take = 2
        });

        return Json(users);
    }
}