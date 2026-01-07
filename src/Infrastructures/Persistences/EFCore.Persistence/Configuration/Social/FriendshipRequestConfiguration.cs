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
    }
}