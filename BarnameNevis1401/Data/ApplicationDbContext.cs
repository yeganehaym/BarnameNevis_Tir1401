using System.Reflection;
using BarnameNevis1401.Data.Configs;
using BarnameNevis1401.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.Data;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<User>().Property(x => x.FirstName).HasMaxLength(100);
        //modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(UserConfig)));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().HaveMaxLength(100);
    }
}