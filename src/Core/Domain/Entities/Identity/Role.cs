using Domain.Common.Contracts;
using Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Identity;

public class Role : AuditableEntity<long>, IAggregateRoot
{
    [MaxLength(256)]
    public string Name { get; private set; } = string.Empty;

    [MaxLength(256)]
    public string NormalizedName { get; private set; } = string.Empty;

    // Factory method for creating a new role
    public static Role Create(string name)
    {
        ValidateRoleData(name);

        return new Role
        {
            Name = name,
            NormalizedName = name.ToUpperInvariant()
        };
    }

    // Update role properties
    public void Update(string name)
    {
        ValidateRoleData(name);

        Name = name;
        NormalizedName = name.ToUpperInvariant();
    }

    // Domain validation
    private static void ValidateRoleData(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Role name cannot be empty");
        }

        if (name.Length > 256)
        {
            throw new DomainValidationException("Role name cannot exceed 256 characters");
        }
    }
}