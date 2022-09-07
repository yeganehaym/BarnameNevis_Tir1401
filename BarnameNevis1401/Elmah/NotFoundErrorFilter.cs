using ElmahCore;

namespace BarnameNevis1401.Elmah;

public class NotFoundErrorFilter:IErrorFilter
{
    public void OnErrorModuleFiltering(object sender, ExceptionFilterEventArgs args)
    {
        if (args.Context is HttpContext)
        {
            var httpContext = (HttpContext)args.Context;
            if (httpContext.Response.StatusCode == 404)
            {
                args.Dismiss();
                //args.DismissForNotifiers(new []{"xnotifer"});
            }
        }
    }
}