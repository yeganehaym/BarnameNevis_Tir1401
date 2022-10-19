using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BarnameNevis1401.Filters;

public class MyAuthorize:Attribute,IAsyncActionFilter
{
    public string Roles { get; set; }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.GetUserId() > 0)
        {
            var claimRoles = context.HttpContext.User.Claims.Where(x => x.Type == "Role").ToList();

            if (string.IsNullOrEmpty(Roles))
            {
                await next();
                return;
            }
            var roles = Roles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (claimRoles!=null && claimRoles.Any(x => roles.Contains(x.Value)))
            {
                await next();
                return;
            }
        }

        var url = context.HttpContext.Request.Path.ToString();
        context.Result = new RedirectToActionResult("login", "auth", new { returnUrl =url  });
    }
}