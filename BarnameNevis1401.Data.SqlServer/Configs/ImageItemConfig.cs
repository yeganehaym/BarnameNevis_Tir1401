using BarnameNevis1401.Domains.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarnameNevis1401.Data.SqlServer.Configs;

public class ImageItemConfig:IEntityTypeConfiguration<ImageItem>
{
    public void Configure(EntityTypeBuilder<ImageItem> builder)
    {
        builder.HasOne(x => x.User)
            .WithMany(x => x.ImageItems)
            .HasForeignKey(x=>x.UserId);
        
    }
}