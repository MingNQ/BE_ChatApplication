using Domain.Entities.Chat;
using EfCore.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Persistence.Configuration.Chat;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations", SchemaNames.Chat);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Type)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(200);

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.LastMessageAt);
    }
}