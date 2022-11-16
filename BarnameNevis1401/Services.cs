using BarnameNevis1401;
using BarnameNevis1401.ApplicationService;
using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using MapsterMapper;

public static class Services
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, ApplicationDbContext>();


        services.AddScoped<IInitializerService,InitializeService>();
        services.AddScoped<IMapper,Mapper>();

        services.AddScoped<IUserService,UserService>();
        services.AddScoped<ITagService,TagService>();
        services.AddScoped<IPaymentService,PaymentService>();
        services.AddScoped<IImageService,ImageService>();
        services.AddScoped<ITest>(options =>
        {
            var service=options.GetService<IUserService>();
            return new Test("ConnctionString");
        });
    }
}