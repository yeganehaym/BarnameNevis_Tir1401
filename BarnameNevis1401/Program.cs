using System.Globalization;
using System.Security.Claims;
using System.Text;
using BarnameNevis1401;
using BarnameNevis1401.Core;
using BarnameNevis1401.CQRS.Commands.Users;
using BarnameNevis1401.Data;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Users;
using BarnameNevis1401.Elmah;
using BarnameNevis1401.Email;
using BarnameNevis1401.Filters;
using BarnameNevis1401.Middleware;
using BarnameNevis1401.Resources;
using DNTCaptcha.Core;
using ElmahCore;
using ElmahCore.Mvc;
using ElmahCore.Mvc.Notifiers;
using ElmahCore.Sql;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using Parbad.Builder;
using Parbad.Gateway.Mellat;
using Parbad.Gateway.Melli;
using Parbad.Gateway.ZarinPal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
    {
        options.CacheProfiles.Add("Profile1",new CacheProfile()
        {
            Duration = 60
        });

        options.Filters.Add(typeof(MyLoggerAttribute));
    })
    .AddRazorRuntimeCompilation()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            return factory.Create(typeof(SharedResource));
        };
    });
   // .AddMvcLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(CreatOrUpdateUserCommand));

builder.Services.Configure<EmailSettings>(x => builder.Configuration.Bind("email",x));

builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("default"));
});

/*
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("Test");
});*/


builder.Services.AddServices();

//==================== Cookie Auth =============================================

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "barnameNevis.Auth";
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.ExpireTimeSpan=TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/auth/login";
        options.Events = new CookieAuthenticationEvents()
        {
            OnValidatePrincipal = async context =>
            {
                var userId = context.Principal.GetUserId();
                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                var user = await userService.FindUserAsync(userId);
                var claimSn = context.Principal.Claims.First(x => x.Type == ClaimTypes.SerialNumber).Value;
                if(user.SerialNumber!=claimSn)
                    context.RejectPrincipal();
                    
            }
        };
    });


//================ JWT (Json Web Token0 Auth =================================
/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents()
        {
            OnTokenValidated = async context =>
            {
                var userId = int.Parse(context.Principal.Claims.First(x => x.Type =="UserId").Value);
                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                var user = await userService.FindUserAsync(userId);
                var sn = context.Principal.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (sn != user.SerialNumber)
                {
                    context.Fail("Serial Number");
                    return;
                }
                context.Success();
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
*/

builder.Services.AddDNTCaptcha(options =>
{
    // options.UseSessionStorageProvider() // -> It doesn't rely on the server or client's times. Also it's the safest one.
    // options.UseMemoryCacheStorageProvider() // -> It relies on the server's times. It's safer than the CookieStorageProvider.
    options.UseCookieStorageProvider(SameSiteMode.Strict) // -> It relies on the server and client's times. It's ideal for scalability, because it doesn't save anything in the server's memory.
        // .UseDistributedCacheStorageProvider() // --> It's ideal for scalability using `services.AddStackExchangeRedisCache()` for instance.
        // .UseDistributedSerializationProvider()

        // Don't set this line (remove it) to use the installed system's fonts (FontName = "Tahoma").
        // Or if you want to use a custom font, make sure that font is present in the wwwroot/fonts folder and also use a good and complete font!
        //.UseCustomFont(Path.Combine(builder.Environment.WebRootPath, "fonts", "IRANSans(FaNum)_Bold.ttf")) // This is optional.
        .AbsoluteExpiration(minutes: 7)
        .ShowThousandsSeparators(false)
        .WithNoise(pixelsDensity: 25, linesCount: 3)
        .WithEncryptionKey("This is my secure key!")
        .InputNames(// This is optional. Change it if you don't like the default names.
            new DNTCaptchaComponent
            {
                CaptchaHiddenInputName = "DNTCaptchaText",
                CaptchaHiddenTokenName = "DNTCaptchaToken",
                CaptchaInputName = "DNTCaptchaInputText"
            })
        .Identifier("dntCaptcha")// This is optional. Change it if you don't like its default name.
        ;
});

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("hangfire"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

builder.Services.AddParbad()
    .ConfigureGateways(gateways =>
    {
        gateways.AddZarinPal()
            .WithAccounts(accounts =>
            {
                accounts.AddInMemory(account =>
                {
                    account.IsSandbox = bool.Parse(builder.Configuration["gateways:zarinpal:sandbox"]);
                    account.MerchantId = builder.Configuration["gateways:zarinpal:merchantId"];
                    account.Name = "Default";
                });

            });
 
    })
    .ConfigureStorage(storage =>
    {
        storage.UseMemoryCache();
    })
    .ConfigureHttpContext(builder =>
    {
        builder.UseDefaultAspNetCore();
    });

builder.Services.AddElmah<SqlErrorLog>(options =>
{
    options.LogPath = Path.Combine(builder.Environment.ContentRootPath, "logs", "elmah");
    options.OnPermissionCheck = context =>
    {
        return true;//(context.User?.Identity?.IsAuthenticated??false) && context.User.IsInRole("Admin");
    };
    options.ConnectionString = builder.Configuration.GetConnectionString("elmah");
    options.Filters.Add(new NotFoundErrorFilter());
    options.Notifiers.Add(new ErrorMailNotifier("test",new EmailOptions()
    {
        
    }));
    
    options.Notifiers.Add(new ElmahEmailNotification());
});

ExcelPackage.LicenseContext = LicenseContext.Commercial;


//================== Localization ======================
var cultures = new List<CultureInfo>()
{
    new CultureInfo("fa"),
    new CultureInfo("en")
};
builder.Services.AddRequestLocalization(options =>
{
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
    options.DefaultRequestCulture = new RequestCulture("fa");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Cors1", options =>
    {
        options.AllowAnyOrigin()
            .Build();
    });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
Stimulsoft.Base.StiLicense.LoadFromFile(Path.Combine(app.Environment.ContentRootPath,"Reports","license.key"));

using (var scope=app.Services.CreateScope())
{
    var initializeService= scope.ServiceProvider.GetRequiredService<IInitializerService>();
    initializeService.Seed();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseElmahExceptionPage();
}

/*app.Use(async (context,next) =>
{
    await context.Response.WriteAsync("BarnameNevis.Dev 1401");
    await next(context);
});
//farghe run and use dar dashtane next hast
app.Run(async (context) =>
{
    await context.Response.WriteAsync("BarnameNevis.Dev 1401");
});
*/

app.Map("/yeganeh", app =>
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Welcome To My Page");
    });
});
/*
app.MapWhen(context =>
{
    var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
    using (var scope = scopeFactory.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return dbContext.Logs.Any(x => x.Ip == context.Connection.RemoteIpAddress.ToString());
    }
   
}, app =>
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Your Ip Is Blocked");
    });
});
*/

//app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseCors("Cors1");


app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath,"ali"))
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<XSSBlock>();

/*var options = new RequestLocalizationOptions();
options.RequestCultureProviders.Insert(0,new CustomLocalizationProvider());
app.UseRequestLocalization(options);*/


app.UseRequestLocalization();

app.UseElmah();
//app.UseXSSBlocker();





app.UseHangfireDashboard();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default2",
    pattern: "{lang=fa}/{controller=Home}/{action=Index}/{id?}");
app.Run();