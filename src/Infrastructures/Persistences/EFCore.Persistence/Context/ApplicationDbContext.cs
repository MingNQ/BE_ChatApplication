using Application.Common.Events;
using Application.Common.Interfaces;
using Domain.Entities.Catalog;
using Domain.Entities.Chat;
using Domain.Entities.Feed;
using Domain.Entities.Identity;
using Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EfCore.Persistence.Context;

public class ApplicationDbContext(
    ICurrentUser currentUser,
    ISerializerService serializer,
    IOptions<DatabaseSettings> dbSettings,
    IEventPublisher events)
    : BaseDbContext(currentUser,
        serializer,
        dbSettings,
        events)
{
    #region Common

    public DbSet<FileStorage> FileStorages => Set<FileStorage>();

    #endregion Common

    #region Audit

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserVerification> UserVerifications => Set<UserVerification>();
    public DbSet<UserClaim> UserClaims => Set<UserClaim>();
    public DbSet<Role> Roles => Set<Role>();

    #endregion Audit

    #region Chat

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
    public DbSet<ConversationReadState> ConversationReadStates => Set<ConversationReadState>();
    public DbSet<ConversationRole> ConversationRoles => Set<ConversationRole>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();

    #endregion Chat

    #region Social

    public DbSet<FriendshipRequest> FriendshipRequests => Set<FriendshipRequest>();
    public DbSet<Follow> Follows => Set<Follow>();

    #endregion Social

    #region Feed

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostAttachment> PostAttachments => Set<PostAttachment>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<PostReaction> PostReactions => Set<PostReaction>();

    #endregion Feed

    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {
        var decimalProps = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => (System.Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal));

        foreach (var property in decimalProps)
        {
            property.SetPrecision(18);
            property.SetScale(2);
        }

        base.OnModelCreating(modelBuilder);
    }
}