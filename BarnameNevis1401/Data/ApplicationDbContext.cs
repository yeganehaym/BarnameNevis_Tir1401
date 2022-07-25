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

}