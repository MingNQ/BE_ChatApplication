using Domain.Common.Contracts;
using Domain.Common.Events;
using Domain.Entities.Catalog;
using Domain.Events;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Domain.Entities.Identity;

public class User : AuditableEntity<long>, IAggregateRoot
{
    // Main constructor with required fields
    public User(string userName, string email, string? firstName, string? lastName)
    {
        SetUserName(userName);
        SetEmail(email);
        FirstName = firstName ?? string.Empty;
        LastName = lastName ?? string.Empty;
        JoinDate = DateTimeOffset.UtcNow;
        LockoutEnabled = true;
        AccessFailedCount = 0;

        // Add domain event for entity creation
        DomainEvents.Add(EntityCreatedEvent.WithEntity(this));
    }

    public static User Create(string userName, string email, string? firstName, string? lastName)
    {
        return new User(userName, email, firstName, lastName);
    }

    [MaxLength(50)]
    public string UserName { get; private set; } = string.Empty;

    [MaxLength(50)]
    public string NormalizedUserName { get; private set; } = string.Empty;

    [MaxLength(256)]
    public string Email { get; private set; } = string.Empty;

    public long? AvatarId { get; private set; }

    public FileStorage? Avatar { get; private set; }

    [MaxLength(256)]
    public string NormalizedEmail { get; private set; } = string.Empty;

    [MaxLength(500)]
    public string PasswordHash { get; private set; } = string.Empty;

    [MaxLength(15)]
    public string PhoneNumber { get; private set; } = string.Empty;

    public bool LockoutEnabled { get; private set; }

    public int AccessFailedCount { get; private set; }

    public DateTimeOffset? LockoutEnd { get; private set; }

    public DateTimeOffset JoinDate { get; private set; }

    [MaxLength(50)]
    public string LastName { get; private set; }

    [MaxLength(50)]
    public string FirstName { get; private set; }

    public bool? IsVerifiedPhone { get; private set; }

    public bool? IsVerifiedEmail { get; private set; }

    [MaxLength(256)]
    public string PasswordResetToken { get; private set; } = string.Empty;

    public DateTimeOffset PasswordResetExpiration { get; private set; }

    [MaxLength(50)]
    public string? RegisterProvider { get; private set; }

    // UserRole collection for DDD aggregate pattern
    private readonly List<UserRole> _userRoles = new();

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    // Domain methods - all state changes go through these methods

    public void SetUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("Username cannot be empty", nameof(userName));
        }

        if (userName.Length < 3)
        {
            throw new ArgumentException("Username must be at least 3 characters", nameof(userName));
        }

        UserName = userName;
        NormalizedUserName = userName.ToUpperInvariant();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        // Basic email validation using regex
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Invalid email format", nameof(email));
        }

        Email = email;
        NormalizedEmail = email.ToUpperInvariant();

        // Reset verification when email changes
        IsVerifiedEmail = false;
    }

    public void SetPassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        }

        PasswordHash = passwordHash;
        DomainEvents.Add(new UserPasswordChangedEvent(this));
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            // Basic phone number validation - can be enhanced
            if (!Regex.IsMatch(phoneNumber.Trim(), @"^\+?[0-9]{10,15}$"))
            {
                throw new ArgumentException("Invalid phone number format", nameof(phoneNumber));
            }
        }

        PhoneNumber = phoneNumber;

        // Reset verification when phone changes
        IsVerifiedPhone = false;
    }

    public void SetAvatar(long avatarId)
    {
        AvatarId = avatarId;
    }

    public void UpdateName(string? firstName, string? lastName)
    {
        FirstName = firstName ?? string.Empty;
        LastName = lastName ?? string.Empty;

        // Add domain event for profile update
        DomainEvents.Add(EntityUpdatedEvent.WithEntity(this));
        DomainEvents.Add(new SyncUserInfoEvent(Id));
    }

    public void VerifyEmail()
    {
        IsVerifiedEmail = true;
        DomainEvents.Add(EntityUpdatedEvent.WithEntity(this));
        DomainEvents.Add(new UserEmailVerifiedEvent(this));
    }

    public void VerifyPhone()
    {
        IsVerifiedPhone = true;
        DomainEvents.Add(EntityUpdatedEvent.WithEntity(this));
        DomainEvents.Add(new UserPhoneVerifiedEvent(this));
    }

    public void LockAccount(TimeSpan duration)
    {
        LockoutEnd = DateTimeOffset.UtcNow.Add(duration);
        DomainEvents.Add(EntityUpdatedEvent.WithEntity(this));
        DomainEvents.Add(new UserLockedOutEvent(this, duration));
    }

    public void UnlockAccount()
    {
        LockoutEnd = null;
        AccessFailedCount = 0;
        DomainEvents.Add(EntityUpdatedEvent.WithEntity(this));
    }

    public bool IsLockedOut()
    {
        return LockoutEnabled && LockoutEnd.HasValue && LockoutEnd > DateTimeOffset.UtcNow;
    }

    public void IncrementAccessFailedCount()
    {
        AccessFailedCount++;
        DomainEvents.Add(EntityUpdatedEvent.WithEntity(this));
    }

    public void ResetAccessFailedCount()
    {
        AccessFailedCount = 0;
        DomainEvents.Add(EntityUpdatedEvent.WithEntity(this));
    }

    public void GeneratePasswordResetToken(string token, TimeSpan expiration)
    {
        PasswordResetToken = token;
        PasswordResetExpiration = DateTimeOffset.UtcNow.Add(expiration);
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = string.Empty;
    }

    public bool IsPasswordResetTokenValid()
    {
        return !string.IsNullOrEmpty(PasswordResetToken) &&
               PasswordResetExpiration > DateTimeOffset.UtcNow;
    }

    public void SetRegisterProvider(string provider)
    {
        RegisterProvider = provider;
    }

    // Role management methods following DDD principles
    public void AddRole(long roleId)
    {
        if (_userRoles.Any(ur => ur.RoleId == roleId))
        {
            return; // Role already exists, do nothing
        }

        var userRole = new UserRole
        {
            UserId = Id,
            RoleId = roleId
        };

        _userRoles.Add(userRole);
    }

    public void AddRoles(IEnumerable<long> roleIds)
    {
        foreach (int roleId in roleIds)
        {
            AddRole(roleId);
        }
    }

    public void RemoveRole(long roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole != null)
        {
            _userRoles.Remove(userRole);
        }
    }

    public void RemoveAllRoles()
    {
        if (_userRoles.Any())
        {
            _userRoles.Clear();
        }
    }

    public void UpdateRoles(IEnumerable<long> roleIds)
    {
        ArgumentNullException.ThrowIfNull(roleIds);

        var newRoleIds = roleIds.ToHashSet();
        var currentRoleIds = _userRoles.Select(ur => ur.RoleId).ToHashSet();

        // Find roles to remove (exist in current but not in new)
        var rolesToRemove = currentRoleIds.Except(newRoleIds).ToList();

        // Find roles to add (exist in new but not in current)
        var rolesToAdd = newRoleIds.Except(currentRoleIds).ToList();

        // Remove roles that are no longer needed
        foreach (var roleId in rolesToRemove)
        {
            RemoveRole(roleId);
        }

        // Add new roles
        foreach (var roleId in rolesToAdd)
        {
            AddRole(roleId);
        }
    }

    public bool HasRole(long roleId)
    {
        return _userRoles.Any(ur => ur.RoleId == roleId);
    }

    public List<long> GetRoleIds()
    {
        return _userRoles.Select(ur => ur.RoleId).ToList();
    }

    // Helper methods
    public string GetFullName()
    {
        return $"{FirstName} {LastName}".Trim();
    }

    public bool IsVerified()
    {
        return IsVerifiedEmail == true && IsVerifiedPhone == true;
    }
}