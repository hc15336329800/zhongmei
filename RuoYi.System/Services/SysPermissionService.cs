using RuoYi.Common.Utils;

namespace RuoYi.System.Services;
public class SysPermissionService : ITransient
{
    private readonly SysRoleService _sysRoleService;
    private readonly SysMenuService _sysMenuService;
    public SysPermissionService(SysRoleService sysRoleService, SysMenuService sysMenuService)
    {
        _sysRoleService = sysRoleService;
        _sysMenuService = sysMenuService;
    }

    public async Task<List<string>> GetRolePermissionAsync(SysUserDto user)
    {
        var roles = new List<string>();
        if (SecurityUtils.IsAdmin(user.UserId))
        {
            roles.Add("admin");
        }
        else
        {
            roles.AddRange(await _sysRoleService.GetRolePermissionByUserId(user.UserId!.Value));
        }

        return roles;
    }

    public List<string> GetMenuPermission(SysUserDto user)
    {
        List<string> perms = new List<string>();
        if (SecurityUtils.IsAdmin(user.UserId))
        {
            perms.Add("*:*:*");
        }
        else
        {
            List<SysRoleDto> roles = user.Roles!;
            if (roles.IsNotEmpty())
            {
                foreach (SysRoleDto role in roles)
                {
                    List<string> rolePerms = _sysMenuService.SelectMenuPermsByRoleId(role.RoleId);
                    role.Permissions = rolePerms;
                    perms.AddRange(rolePerms);
                }
            }
            else
            {
                perms.AddRange(_sysMenuService.SelectMenuPermsByUserId(user.UserId!.Value));
            }
        }

        return perms;
    }
}