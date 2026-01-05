using Domain.Entities.Chat;
using EfCore.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Persistence.Configuration.Chat;

public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        builder.ToTable("ConversationMembers", SchemaNames.Chat);

        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.ConversationId)
            .IsRequired();

        builder.Property(cm => cm.ConversationRoleId)
            .IsRequired();

        builder.Property(cm => cm.UserId)
            .IsRequired();

        builder.HasOne(cm => cm.Conversation)
            .WithMany(cm => cm.Members)
            .HasForeignKey(cm => cm.ConversationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.ConversationRole)
            .WithMany()
            .HasForeignKey(cm => cm.ConversationRoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cm => cm.User)
            .WithMany()
            .HasForeignKey(cm => cm.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cm => cm.AddedByUser)
            .WithMany()
            .HasForeignKey(cm => cm.AddedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}