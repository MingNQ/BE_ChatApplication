using Domain.Entities.Chat;
using EfCore.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Persistence.Configuration.Chat;

public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.ToTable("MessageAttachments", SchemaNames.Chat);

        builder.HasKey(ma => ma.Id);

        builder.Property(ma => ma.AttachmentType)
            .IsRequired();

        builder.HasOne(ma => ma.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(ma => ma.MessageId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ma => ma.FileStorage)
            .WithMany()
            .HasForeignKey(ma => ma.FileStorageId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}