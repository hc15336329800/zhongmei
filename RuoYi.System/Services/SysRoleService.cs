using RuoYi.Common.Constants;
using RuoYi.Common.Interceptors;
using RuoYi.Common.Utils;
using RuoYi.Framework.Exceptions;
using RuoYi.Framework.Interceptors;
using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysRoleService : BaseService<SysRole, SysRoleDto>, ITransient
{
    private readonly ILogger<SysRoleService> _logger;
    private readonly SysRoleRepository _sysRoleRepository;
    private readonly SysRoleMenuRepository _sysRoleMenuRepository;
    private readonly SysRoleDeptRepository _sysRoleDeptRepository;
    private readonly SysUserRoleRepository _sysUserRoleRepository;
    public SysRoleService(ILogger<SysRoleService> logger, SysRoleRepository sysRoleRepository, SysRoleMenuRepository sysRoleMenuRepository, SysRoleDeptRepository sysRoleDeptRepository, SysUserRoleRepository sysUserRoleRepository)
    {
        BaseRepo = sysRoleRepository;
        _logger = logger;
        _sysRoleRepository = sysRoleRepository;
        _sysRoleMenuRepository = sysRoleMenuRepository;
        _sysRoleDeptRepository = sysRoleDeptRepository;
        _sysUserRoleRepository = sysUserRoleRepository;
    }

    public virtual async Task<SqlSugarPagedList<SysRoleDto>> GetPagedRoleListAsync(SysRoleDto dto)
    {
        return await _sysRoleRepository.GetDtoPagedListAsync(dto);
    }

    public virtual async Task<List<SysRoleDto>> GetRoleListAsync(SysRoleDto dto)
    {
        return await _sysRoleRepository.GetDtoListAsync(dto);
    }

    public async Task<SysRoleDto> GetDtoAsync(long id)
    {
        var entity = await GetAsync(id);
        var dto = entity.Adapt<SysRoleDto>();
        return dto;
    }

    public async Task<SysRole> GetAsync(long id)
    {
        return await _sysRoleRepository.FirstOrDefaultAsync(e => e.RoleId == id);
    }

    public async Task<List<string>> GetRolePermissionByUserId(long userId)
    {
        List<SysRole> perms = await _sysRoleRepository.GetListAsync(new SysRoleDto { UserId = userId });
        List<string> permsSet = new List<string>();
        foreach (SysRole perm in perms)
        {
            if (perm != null)
            {
                permsSet.AddRange(perm.RoleKey!.Trim().Split(","));
            }
        }

        return permsSet;
    }

    public List<SysRoleDto> GetRolesByUserName(string userName)
    {
        var dto = new SysRoleDto
        {
            DelFlag = DelFlag.No,
            UserName = userName
        };
        return this.GetDtoList(dto);
    }

    public async Task<List<SysRoleDto>> GetRolesByUserIdAsync(long userId)
    {
        var dto = new SysRoleDto
        {
            DelFlag = DelFlag.No
        };
        var roles = await this.GetDtoListAsync(dto);
        foreach (var role in roles)
        {
            if (role.UserId.Equals(userId))
            {
                role.Flag = true;
            }
        }

        return roles;
    }

    public async Task<bool> CheckRoleNameUniqueAsync(SysRoleDto dto)
    {
        long roleId = dto.RoleId;
        SysRole info = await _sysRoleRepository.GetByRoleNameAsync(dto.RoleName!);
        if (info != null && info.RoleId != roleId)
        {
            return UserConstants.NOT_UNIQUE;
        }

        return UserConstants.UNIQUE;
    }

    public async Task<bool> CheckRoleKeyUniqueAsync(SysRoleDto dto)
    {
        long roleId = dto.RoleId;
        SysRole info = await _sysRoleRepository.GetByRoleKeyAsync(dto.RoleKey!);
        if (info != null && info.RoleId != roleId)
        {
            return UserConstants.NOT_UNIQUE;
        }

        return UserConstants.UNIQUE;
    }

    public void CheckRoleAllowed(SysRoleDto role)
    {
        if (role.RoleId > 0 && SecurityUtils.IsAdminRole(role.RoleId))
        {
            throw new ServiceException("不允许操作超级管理员角色");
        }
    }

    public async Task CheckRoleDataScopeAsync(long roleId)
    {
        if (!SecurityUtils.IsAdmin())
        {
            SysRoleDto dto = new SysRoleDto
            {
                RoleId = roleId
            };
            List<SysRole> roles = await _sysRoleRepository.GetListAsync(dto);
            if (roles.IsEmpty())
            {
                throw new ServiceException("没有权限访问角色数据！");
            }
        }
    }

#region 新增
    [Transactional]
    public virtual async Task<int> InsertRoleAsync(SysRoleDto role)
    {
        role.DelFlag = DelFlag.No;
        await _sysRoleRepository.InsertAsync(role);
        return await InsertRoleMenuAsync(role);
    }

#endregion
    public async Task<int> InsertRoleMenuAsync(SysRoleDto role)
    {
        int rows = 1;
        List<SysRoleMenu> list = new List<SysRoleMenu>();
        foreach (long menuId in role.MenuIds)
        {
            SysRoleMenu rm = new SysRoleMenu
            {
                RoleId = role.RoleId,
                MenuId = menuId
            };
            list.Add(rm);
        }

        if (list.Count > 0)
        {
            rows = await _sysRoleMenuRepository.InsertAsync(list);
        }

        return rows;
    }

#region 修改
    public async Task<int> UpdateRoleAsync(SysRoleDto dto)
    {
        var role = dto.Adapt<SysRole>();
        await _sysRoleRepository.UpdateAsync(role, true);
        await _sysRoleMenuRepository.DeleteByRoleIdAsync(role.RoleId);
        return await InsertRoleMenuAsync(dto);
    }

#endregion
#region 删除
    public async Task<int> DeleteRoleByIdsAsync(List<long> roleIds)
    {
        foreach (long roleId in roleIds)
        {
            CheckRoleAllowed(new SysRoleDto { RoleId = roleId });
            await CheckRoleDataScopeAsync(roleId);
            if (await _sysUserRoleRepository.CountUserRoleByRoleIdAsync(roleId) > 0)
            {
                SysRole role = await this.GetAsync(roleId);
                throw new ServiceException($"{role.RoleName}已分配,不能删除");
            }
        }

        await _sysRoleMenuRepository.DeleteByRoleIdsAsync(roleIds.ToList());
        await _sysRoleDeptRepository.DeleteByRoleIdsAsync(roleIds.ToList());
        return await _sysRoleRepository.DeleteByRoleIdsAsync(roleIds);
    }

#endregion
    public async Task<int> UpdateRoleStatusAsync(SysRoleDto role)
    {
        return await _sysRoleRepository.UpdateAsync(role, true);
    }

#region 权限
    public async Task<int> AuthDataScopeAsync(SysRoleDto dto)
    {
        var role = dto.Adapt<SysRole>();
        await _sysRoleRepository.UpdateAsync(role, true);
        await _sysRoleDeptRepository.DeleteByRoleIdAsync(role.RoleId);
        return await InsertRoleDeptAsync(dto);
    }

    public async Task<int> InsertRoleDeptAsync(SysRoleDto role)
    {
        int rows = 1;
        List<SysRoleDept> list = new List<SysRoleDept>();
        foreach (long deptId in role.DeptIds)
        {
            SysRoleDept rd = new SysRoleDept
            {
                RoleId = role.RoleId,
                DeptId = deptId
            };
            list.Add(rd);
        }

        if (list.Count > 0)
        {
            rows = await _sysRoleDeptRepository.InsertAsync(list);
        }

        return rows;
    }

    public async Task<int> DeleteAuthUserAsync(SysUserRoleDto userRole)
    {
        return await _sysUserRoleRepository.DeleteUserRoleInfoAsync(userRole.RoleId, userRole.UserId);
    }

    public async Task<int> DeleteAuthUserBathAsync(SysUserRoleDto userRole)
    {
        return await _sysUserRoleRepository.DeleteUserRoleInfoAsync(userRole.RoleId, userRole.UserIds);
    }

    public async Task<int> InsertAuthUsersAsync(long roleId, List<long> userIds)
    {
        List<SysUserRole> list = new List<SysUserRole>();
        foreach (long userId in userIds)
        {
            SysUserRole ur = new SysUserRole
            {
                UserId = userId,
                RoleId = roleId
            };
            list.Add(ur);
        }

        return await _sysUserRoleRepository.InsertAsync(list);
    }
#endregion
}