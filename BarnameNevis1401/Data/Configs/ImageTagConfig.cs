using BarnameNevis1401.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarnameNevis1401.Data.Configs;

public class ImageTagConfig:IEntityTypeConfiguration<ImageTag>
{
    public void Configure(EntityTypeBuilder<ImageTag> builder)
    {
        builder.HasOne(x => x.Tag)
            .WithMany(x => x.ImageTags)
            .HasForeignKey(x => x.TagId);
        
        builder.HasOne(x => x.ImageItem)
            .WithMany(x => x.ImageTags)
            .HasForeignKey(x => x.ImageItemId);

        
        builder.HasKey(x => new { x.TagId, x.ImageItemId });
    }
}