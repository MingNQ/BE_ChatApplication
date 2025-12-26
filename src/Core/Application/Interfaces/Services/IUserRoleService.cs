using Application.Common.Models;
using Application.Dto.Persistence.Catalog.UserRole;

namespace Application.Interfaces.Services;

public interface IUserRoleService
{
    Task DeleteById(int userId, int roleId);

    Task<PaginationResponse<UserRoleDto>> GetAll(GetUserRoleInput input);

    Task<UserRoleDto> GetById(int userId, int roleId);

    Task<UserRoleDto> Insert(UserRoleDto entity);

    Task Update(int userId, int roleId, UserRoleDto entity);
}