using BarnameNevis1401.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarnameNevis1401.Data.Configs;

public class UserConfig:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);
        builder.Property(x => x.Username).HasMaxLength(50);
        builder.Property(x => x.Password).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.Mobile).HasMaxLength(11);
        builder.HasIndex(x => x.Username).IsUnique();
        //builder.HasKey(x => new[] { x.Mobile,x.Email });
        //builder.Ignore(x => x.Email);
    }
}