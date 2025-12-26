using Application.Common.Pagination;

namespace Application.Dto.Persistence.Catalog.UserRole;

public class GetUserRoleInput : PagedInputDto
{
    public string KeySearch { get; set; } = string.Empty;
}