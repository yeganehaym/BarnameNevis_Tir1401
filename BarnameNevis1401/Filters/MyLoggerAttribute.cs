using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace BarnameNevis1401.Filters;

public class MyLoggerAttribute:Attribute,IAsyncActionFilter
{
    private ApplicationDbContext _applicationDbContext;
  
    public MyLoggerAttribute(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logs = _applicationDbContext.Set<Log>();
        var log = new Log();
        log.Ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        var userId = context.HttpContext.User.GetUserId();
        log.UserId = userId > 0 ? userId : null;
        log.Agent = context.HttpContext.Request.Headers["user-agent"].ToString();
        log.Controller = context.HttpContext.Request.RouteValues["Controller"]?.ToString();
        log.Action = context.HttpContext.Request.RouteValues["Action"]?.ToString();
        log.RequestTime=DateTime.Now;
        log.Parameters = String.Empty;

        var dictionary = new Dictionary<string, string>();
        var routesValues = context.HttpContext.Request.RouteValues;
        foreach (var item in routesValues)
        {
            if(item.Key.ToLower()=="Controller".ToLower() || item.Key.ToLower()=="Action".ToLower())
                continue;
            dictionary.Add(item.Key,item.Value.ToString());
        }

        foreach (var item in context.HttpContext.Request.Query)
        {
            dictionary.Add(item.Key,item.Value);
        }

        if (context.HttpContext.Request.Method.ToUpper() == "POST" && context.HttpContext.Request.Form != null)
        {
            var keys = context.HttpContext.Request.Form.Keys;
            foreach (var key in keys)
            {
                var value = context.HttpContext.Request.Form[key].ToString();
                dictionary.Add(key,value);
            }
        }
        

        var json = JsonConvert.SerializeObject(dictionary);
        log.Parameters = json;
        
        await logs.AddAsync(log);
        await _applicationDbContext.SaveChangesAsync();
        await next();

    }
}