using System.Reflection;
using BarnameNevis1401.Data.SqlServer.Configs;
using BarnameNevis1401.Data.SqlServer.Conversions;
using BarnameNevis1401.Domains;
using BarnameNevis1401.Domains.Images;
using BarnameNevis1401.Domains.Payments;
using BarnameNevis1401.Domains.Users;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.Data.SqlServer;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ImageItem> ImageItems { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ImageTag> ImageTags { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<User>().Property(x => x.FirstName).HasMaxLength(100);
        //modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(UserConfig)));

       // modelBuilder.Entity<Tag>().HasQueryFilter(x => x.IsRemoved == false);

       var models =
           Enumerable.ToList<Type>(modelBuilder
                   .Model
                   .GetEntityTypes()
                   .Where(x => x.ClrType.BaseType == typeof(BaseEntity))
                   .Select(x=>x.ClrType));

       var method = typeof(ApplicationDbContext)
           .GetMethod("SetRemoveQueryFilter");
       
       foreach (var model in models)
       {
           var genericMethod = method.MakeGenericMethod(model);
           genericMethod.Invoke(this, new[] { modelBuilder });
       }
      /* var converter = new ValueConverter<TimeSpan, long>(
           v => v.Ticks,
           v => new TimeSpan(v));

       modelBuilder.Entity<ImageItem>().Property(x => x.UploadTimeDuration).HasConversion(converter);
       */
    }
    
    

    public void SetRemoveQueryFilter<T>(ModelBuilder modelBuilder) where T:BaseEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(x => x.IsRemoved == false);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().HaveMaxLength(100);
        
        
        configurationBuilder.Properties<TimeSpan>().HaveConversion<TimeSpanConverter>();
        configurationBuilder.Properties<string[]>().HaveConversion<StringArrayConverter>();
    }
}