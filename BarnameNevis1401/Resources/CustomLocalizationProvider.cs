using Microsoft.AspNetCore.Localization;

namespace BarnameNevis1401.Resources;

public class CustomLocalizationProvider:RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var lang = httpContext.Request.RouteValues["lang"];
        var language = "fa";
        if (lang!=null || string.IsNullOrEmpty(lang.ToString())==false)
        {
            language = lang.ToString();
        }

        return null;
    }
}