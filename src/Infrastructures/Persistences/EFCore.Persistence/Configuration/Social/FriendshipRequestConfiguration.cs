using Domain.Entities.Social;
using EfCore.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Persistence.Configuration.Social;

public class FriendshipRequestConfiguration : IEntityTypeConfiguration<FriendshipRequest>
{
    public void Configure(EntityTypeBuilder<FriendshipRequest> builder)
    {
        builder.ToTable("FriendshipRequests", SchemaNames.Social);

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Friend)
            .WithMany()
            .HasForeignKey(x => x.FriendId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}