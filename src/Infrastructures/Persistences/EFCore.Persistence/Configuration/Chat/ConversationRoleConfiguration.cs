using Domain.Entities.Chat;
using EfCore.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Persistence.Configuration.Chat;

public class ConversationRoleConfiguration : IEntityTypeConfiguration<ConversationRole>
{
    public void Configure(EntityTypeBuilder<ConversationRole> builder)
    {
        builder.ToTable("ConversationRoles", SchemaNames.Chat);

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(cr => cr.NormalizedName)
            .IsRequired()
            .HasMaxLength(256);
    }
}