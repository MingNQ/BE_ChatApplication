using Domain.Entities.Social;
using EfCore.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Persistence.Configuration.Social;

public class UserPresenceSnapshotConfiguration : IEntityTypeConfiguration<UserPresenceSnapshot>
{
    public void Configure(EntityTypeBuilder<UserPresenceSnapshot> builder)
    {
        builder.ToTable("UserPresenceSnapshots", SchemaNames.Social);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();
    }
}