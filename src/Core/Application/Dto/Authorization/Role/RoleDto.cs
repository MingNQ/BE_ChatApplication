using Application.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Authorization.Role;

public class RoleDto : IDto
{
    public int Id { get; set; }

    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(256)]
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// A random value that should change whenever a role is persisted to the store
    /// </summary>
    public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
}