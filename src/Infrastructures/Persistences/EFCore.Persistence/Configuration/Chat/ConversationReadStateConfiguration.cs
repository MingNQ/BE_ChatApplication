using Domain.Entities.Chat;
using EfCore.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Persistence.Configuration.Chat;

public class ConversationReadStateConfiguration : IEntityTypeConfiguration<ConversationReadState>
{
    public void Configure(EntityTypeBuilder<ConversationReadState> builder)
    {
        builder.ToTable("ConversationReadStates", SchemaNames.Chat);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ConversationId)
            .IsRequired();

        builder.Property(e => e.ReadAt)
            .IsRequired();

        builder.HasOne(e => e.Conversation)
            .WithMany()
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Conversation)
            .WithOne()
            .HasForeignKey<ConversationReadState>(e => e.LastReadMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.ConversationId, e.UserId, e.LastReadMessageId }).IsUnique();
    }
}