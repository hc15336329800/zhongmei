using RuoYi.Common.Data;
using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using RuoYi.Framework;
using RuoYi.System.Services;

namespace RuoYi.System.Controllers
{
    [ApiDescriptionSettings("System")]
    [Route("system/user")]
    public class SysUserController : ControllerBase
    {
        private readonly ILogger<SysUserController> _logger;
        private readonly SysUserService _sysUserService;
        private readonly SysRoleService _sysRoleService;
        private readonly SysPostService _sysPostService;
        private readonly SysDeptService _sysDeptService;
        public SysUserController(ILogger<SysUserController> logger, SysUserService sysUserService, SysRoleService sysRoleService, SysPostService sysPostService, SysDeptService sysDeptService)
        {
            _logger = logger;
            _sysUserService = sysUserService;
            _sysRoleService = sysRoleService;
            _sysPostService = sysPostService;
            _sysDeptService = sysDeptService;
        }

        [HttpGet("list")]
        [AppAuthorize("system:user:list")]
        public async Task<SqlSugarPagedList<SysUser>> GetUserList([FromQuery] SysUserDto dto)
        {
            return await _sysUserService.GetPagedUserListAsync(dto);
        }

        [HttpGet("")]
        [HttpGet("{userId}")]
        [AppAuthorize("system:user:query")]
        public async Task<AjaxResult> GetInfo(long? userId)
        {
            await _sysUserService.CheckUserDataScope(userId);
            var roles = await _sysRoleService.GetListAsync(new SysRoleDto());
            var posts = await _sysPostService.GetListAsync(new SysPostDto());
            AjaxResult ajax = AjaxResult.Success();
            ajax.Add("roles", SecurityUtils.IsAdmin(userId) ? roles : roles.Where(r => !SecurityUtils.IsAdminRole(r.RoleId)));
            ajax.Add("posts", posts);
            if (userId.HasValue && userId > 0)
            {
                var user = await _sysUserService.GetDtoAsync(userId);
                ajax.Add(AjaxResult.DATA_TAG, user);
                ajax.Add("postIds", _sysPostService.GetPostIdsListByUserId(userId.Value));
                ajax.Add("roleIds", user.Roles.Select(x => x.RoleId).ToList());
            }

            return ajax;
        }

        [HttpPost("")]
        [AppAuthorize("system:user:add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [Log(Title = "用户管理", BusinessType = BusinessType.INSERT)]
        public async Task<AjaxResult> Add([FromBody] SysUserDto user)
        {
            if (!await _sysUserService.CheckUserNameUniqueAsync(user))
            {
                return AjaxResult.Error("新增用户'" + user.UserName + "'失败，登录账号已存在");
            }
            else if (!string.IsNullOrEmpty(user.Phonenumber) && !await _sysUserService.CheckPhoneUniqueAsync(user))
            {
                return AjaxResult.Error("新增用户'" + user.UserName + "'失败，手机号码已存在");
            }
            else if (!string.IsNullOrEmpty(user.Email) && !await _sysUserService.CheckEmailUniqueAsync(user))
            {
                return AjaxResult.Error("新增用户'" + user.UserName + "'失败，邮箱账号已存在");
            }

            var data = _sysUserService.InsertUser(user);
            return AjaxResult.Success(data);
        }

        [HttpPut("")]
        [AppAuthorize("system:user:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [Log(Title = "用户管理", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] SysUserDto user)
        {
            _sysUserService.CheckUserAllowed(user);
            await _sysUserService.CheckUserDataScope(user.UserId ?? 0);
            if (!await _sysUserService.CheckUserNameUniqueAsync(user))
            {
                return AjaxResult.Error("修改用户'" + user.UserName + "'失败，登录账号已存在");
            }
            else if (!string.IsNullOrEmpty(user.Phonenumber) && !await _sysUserService.CheckPhoneUniqueAsync(user))
            {
                return AjaxResult.Error("修改用户'" + user.UserName + "'失败，手机号码已存在");
            }
            else if (!string.IsNullOrEmpty(user.Email) && !await _sysUserService.CheckEmailUniqueAsync(user))
            {
                return AjaxResult.Error("修改用户'" + user.UserName + "'失败，邮箱账号已存在");
            }

            var data = _sysUserService.UpdateUser(user);
            return AjaxResult.Success(data);
        }

        [HttpDelete("{ids}")]
        [AppAuthorize("system:user:remove")]
        [Log(Title = "用户管理", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(string ids)
        {
            var userIds = ids.SplitToList<long>();
            if (userIds.Contains(SecurityUtils.GetUserId()))
            {
                return AjaxResult.Error("当前用户不能删除");
            }

            var data = await _sysUserService.DeleteUserByIdsAsync(userIds);
            return AjaxResult.Success(data);
        }

        [HttpPut("resetPwd")]
        [AppAuthorize("system:user:resetPwd")]
        [Log(Title = "用户管理", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> ResetPwd([FromBody] SysUserDto user)
        {
            _sysUserService.CheckUserAllowed(user);
            await _sysUserService.CheckUserDataScope(user.UserId ?? 0);
            var data = _sysUserService.ResetPwd(user);
            return AjaxResult.Success(data);
        }

        [HttpPut("changeStatus")]
        [AppAuthorize("system:user:edit")]
        [Log(Title = "用户管理", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> ChangeStatus([FromBody] SysUserDto user)
        {
            _sysUserService.CheckUserAllowed(user);
            await _sysUserService.CheckUserDataScope(user.UserId ?? 0);
            var data = await _sysUserService.UpdateUserStatus(user);
            return AjaxResult.Success(data);
        }

        [HttpGet("authRole/{userId}")]
        [AppAuthorize("system:user:query")]
        public async Task<AjaxResult> GetAuthRole(long userId)
        {
            var user = await _sysUserService.GetDtoAsync(userId);
            var roles = await _sysRoleService.GetRolesByUserIdAsync(userId);
            AjaxResult ajax = AjaxResult.Success();
            ajax.Add("user", user);
            ajax.Add("roles", SecurityUtils.IsAdmin(userId) ? roles : roles.Where(r => !SecurityUtils.IsAdminRole(r.RoleId)));
            return ajax;
        }

        [HttpPut("authRole")]
        [AppAuthorize("system:user:edit")]
        [Log(Title = "用户管理", BusinessType = BusinessType.GRANT)]
        public async Task<AjaxResult> InsertAuthRole(long userId, string roleIds)
        {
            var rIds = roleIds.SplitToList<long>();
            await _sysUserService.CheckUserDataScope(userId);
            _sysUserService.InsertUserAuth(userId, rIds);
            return AjaxResult.Success();
        }

        [HttpGet("deptTree")]
        [AppAuthorize("system:user:list")]
        public async Task<AjaxResult> GetDeptTree([FromQuery] SysDeptDto dept)
        {
            var data = await _sysDeptService.GetDeptTreeListAsync(dept);
            return AjaxResult.Success(data);
        }

        [HttpPost("importData")]
        [AppAuthorize("system:user:import")]
        [Log(Title = "用户管理", BusinessType = BusinessType.IMPORT)]
        public async Task<AjaxResult> Import([FromForm] IFormFile file, bool updateSupport)
        {
            var stream = new MemoryStream();
            file.CopyTo(stream);
            var list = await ExcelUtils.ImportAllAsync<SysUserDto>(stream);
            var msg = await _sysUserService.ImportDtosAsync(list, updateSupport, SecurityUtils.GetUsername());
            return AjaxResult.Success(msg);
        }

        [HttpPost("importTemplate")]
        [AppAuthorize("system:user:import")]
        public async Task DownloadImportTemplate()
        {
            await ExcelUtils.GetImportTemplateAsync<SysUserDto>(App.HttpContext.Response, "用户数据");
        }

        [HttpPost("export")]
        [AppAuthorize("system:user:export")]
        [Log(Title = "用户管理", BusinessType = BusinessType.EXPORT)]
        public async Task Export(SysUserDto dto)
        {
            var list = await _sysUserService.GetUserListAsync(dto);
            var dtos = _sysUserService.ToDtos(list);
            await ExcelUtils.ExportAsync(App.HttpContext.Response, dtos);
        }
    }
}