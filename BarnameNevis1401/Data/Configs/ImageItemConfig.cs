using BarnameNevis1401.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarnameNevis1401.Data.Configs;

public class ImageItemConfig:IEntityTypeConfiguration<ImageItem>
{
    public void Configure(EntityTypeBuilder<ImageItem> builder)
    {
        builder.HasOne(x => x.User)
            .WithMany(x => x.ImageItems)
            .HasForeignKey(x=>x.UserId);
        
    }
}