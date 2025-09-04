using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.System.Services;

namespace RuoYi.System.Controllers
{
    [ApiDescriptionSettings("System")]
    [Route("system/role")]
    public class SysRoleController : ControllerBase
    {
        private readonly ILogger<SysRoleController> _logger;
        private readonly TokenService _tokenService;
        private readonly SysUserService _sysUserService;
        private readonly SysRoleService _sysRoleService;
        private readonly SysDeptService _sysDeptService;
        private readonly SysPermissionService _sysPermissionService;
        public SysRoleController(ILogger<SysRoleController> logger, TokenService tokenService, SysUserService sysUserService, SysRoleService sysRoleService, SysDeptService sysDeptService, SysPermissionService sysPermissionService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _sysUserService = sysUserService;
            _sysRoleService = sysRoleService;
            _sysDeptService = sysDeptService;
            _sysPermissionService = sysPermissionService;
        }

        [HttpGet("list")]
        [AppAuthorize("system:role:list")]
        public async Task<SqlSugarPagedList<SysRoleDto>> GetSysRoleList([FromQuery] SysRoleDto dto)
        {
            return await _sysRoleService.GetPagedRoleListAsync(dto);
        }

        [HttpGet("{id}")]
        [AppAuthorize("system:role:query")]
        public async Task<AjaxResult> Get(long id)
        {
            await _sysRoleService.CheckRoleDataScopeAsync(id);
            var data = await _sysRoleService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        [HttpPost("")]
        [AppAuthorize("system:role:add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [Log(Title = "角色管理", BusinessType = BusinessType.INSERT)]
        public async Task<AjaxResult> Add([FromBody] SysRoleDto role)
        {
            if (!await _sysRoleService.CheckRoleNameUniqueAsync(role))
            {
                return AjaxResult.Error($"新增角色'{role.RoleName}失败，角色名称已存在");
            }
            else if (!await _sysRoleService.CheckRoleKeyUniqueAsync(role))
            {
                return AjaxResult.Error($"新增角色'{role.RoleName}失败，角色权限已存在");
            }

            var data = await _sysRoleService.InsertRoleAsync(role);
            return AjaxResult.Success(data);
        }

        [HttpPut("")]
        [AppAuthorize("system:role:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [Log(Title = "角色管理", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] SysRoleDto role)
        {
            _sysRoleService.CheckRoleAllowed(role);
            await _sysRoleService.CheckRoleDataScopeAsync(role.RoleId);
            if (!await _sysRoleService.CheckRoleNameUniqueAsync(role))
            {
                return AjaxResult.Error($"修改角色'{role.RoleName}'失败，角色名称已存在");
            }
            else if (!await _sysRoleService.CheckRoleKeyUniqueAsync(role))
            {
                return AjaxResult.Error($"修改角色'{role.RoleName}'失败，角色权限已存在");
            };
            if (await _sysRoleService.UpdateRoleAsync(role) > 0)
            {
                LoginUser loginUser = SecurityUtils.GetLoginUser();
                if (loginUser.User != null && !SecurityUtils.IsAdmin(loginUser.User))
                {
                    loginUser.Permissions = _sysPermissionService.GetMenuPermission(loginUser.User);
                    loginUser.User = await _sysUserService.GetDtoByUsernameAsync(loginUser.User.UserName!);
                    _tokenService.SetLoginUser(loginUser);
                }

                return AjaxResult.Success();
            }

            return AjaxResult.Error("修改角色'" + role.RoleName + "'失败，请联系管理员");
        }

        [HttpPut("dataScope")]
        [Log(Title = "角色管理", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> SaveDataScope([FromBody] SysRoleDto role)
        {
            _sysRoleService.CheckRoleAllowed(role);
            await _sysRoleService.CheckRoleDataScopeAsync(role.RoleId);
            var data = await _sysRoleService.AuthDataScopeAsync(role);
            return AjaxResult.Success(data);
        }

        [HttpPut("changeStatus")]
        [Log(Title = "角色管理", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> ChangeStatus([FromBody] SysRoleDto role)
        {
            _sysRoleService.CheckRoleAllowed(role);
            await _sysRoleService.CheckRoleDataScopeAsync(role.RoleId);
            var data = await _sysRoleService.UpdateRoleStatusAsync(role);
            return AjaxResult.Success(data);
        }

        [HttpDelete("{ids}")]
        [AppAuthorize("system:role:remove")]
        [Log(Title = "角色管理", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(List<long> ids)
        {
            var data = await _sysRoleService.DeleteRoleByIdsAsync(ids);
            return AjaxResult.Success(data);
        }

        [HttpPost("export")]
        [AppAuthorize("system:role:export")]
        [Log(Title = "角色管理", BusinessType = BusinessType.EXPORT)]
        public async Task Export(SysRoleDto dto)
        {
            var list = await _sysRoleService.GetRoleListAsync(dto);
            await ExcelUtils.ExportAsync(App.HttpContext.Response, list);
        }

        [HttpPost("optionselect")]
        [AppAuthorize("system:role:query")]
        public async Task<AjaxResult> OptionSelect()
        {
            var data = await _sysRoleService.GetListAsync(new SysRoleDto());
            return AjaxResult.Success(data);
        }

        [HttpGet("authUser/allocatedList")]
        [AppAuthorize("system:role:list")]
        public async Task<SqlSugarPagedList<SysUserDto>> GetAllocatedList([FromQuery] SysUserDto dto)
        {
            return await _sysUserService.GetPagedAllocatedListAsync(dto);
        }

        [HttpGet("authUser/unallocatedList")]
        [AppAuthorize("system:role:list")]
        public async Task<SqlSugarPagedList<SysUserDto>> GetUnallocatedList([FromQuery] SysUserDto dto)
        {
            return await _sysUserService.GetPagedUnallocatedListAsync(dto);
        }

        [HttpPut("authUser/cancel")]
        [AppAuthorize("system:role:edit")]
        [Log(Title = "角色管理", BusinessType = BusinessType.GRANT)]
        public async Task<AjaxResult> CancelAuthUser([FromBody] SysUserRoleDto dto)
        {
            var data = await _sysRoleService.DeleteAuthUserAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPut("authUser/cancelAll")]
        [AppAuthorize("system:role:edit")]
        [Log(Title = "角色管理", BusinessType = BusinessType.GRANT)]
        public async Task<AjaxResult> CancelAuthUserBath([FromQuery] SysUserRoleDto dto)
        {
            var data = await _sysRoleService.DeleteAuthUserBathAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPut("authUser/selectAll")]
        [AppAuthorize("system:role:edit")]
        public async Task<AjaxResult> SaveAuthUserAll([FromQuery] SysUserRoleDto dto)
        {
            await _sysRoleService.CheckRoleDataScopeAsync(dto.RoleId);
            var data = await _sysRoleService.InsertAuthUsersAsync(dto.RoleId, dto.UserIds);
            return AjaxResult.Success(data);
        }

        [HttpGet("deptTree/{roleId}")]
        public async Task<AjaxResult> GetDeptTree(long roleId)
        {
            AjaxResult ajax = AjaxResult.Success();
            ajax.Add("checkedKeys", await _sysDeptService.GetDeptListByRoleIdAsync(roleId));
            ajax.Add("depts", await _sysDeptService.GetDeptTreeListAsync(new SysDeptDto()));
            return ajax;
        }
    }
}