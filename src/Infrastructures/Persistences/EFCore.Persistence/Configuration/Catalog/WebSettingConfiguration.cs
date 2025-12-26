using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Persistence.Configuration.Catalog;

public class WebSettingConfiguration : IEntityTypeConfiguration<WebSetting>
{
    public void Configure(EntityTypeBuilder<WebSetting> builder)
    {
        builder.ToTable("WebSettings", SchemaNames.Catalog);
    }
}