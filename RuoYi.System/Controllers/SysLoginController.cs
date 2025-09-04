using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.System.Services;

namespace RuoYi.Admin
{
    [ApiDescriptionSettings("System")]
    public class SysLoginController : ControllerBase
    {
        private readonly ILogger<SysLoginController> _logger;
        private readonly TokenService _tokenService;
        private readonly SysLoginService _sysLoginService;
        private readonly SysPermissionService _sysPermissionService;
        private readonly SysMenuService _sysMenuService;
        private readonly SysLogininforService _sysLogininforService;
        public SysLoginController(ILogger<SysLoginController> logger, TokenService tokenService, SysLoginService sysLoginService, SysPermissionService sysPermissionService, SysMenuService sysMenuService, SysLogininforService sysLogininforService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _sysLoginService = sysLoginService;
            _sysPermissionService = sysPermissionService;
            _sysMenuService = sysMenuService;
            _sysLogininforService = sysLogininforService;
        }

        [HttpPost("/login")]
        public async Task<AjaxResult> Login([FromBody] LoginBody loginBody)
        {
            AjaxResult ajax = AjaxResult.Success();
            string token = await _sysLoginService.LoginAsync(loginBody.Username, loginBody.Password, loginBody.Code, loginBody.Uuid);
            ajax.Add(Constants.TOKEN, token);
            return ajax;
        }

        [HttpPost("/logout")]
        public AjaxResult Logout()
        {
            LoginUser loginUser = _tokenService.GetLoginUser(App.HttpContext.Request);
            if (loginUser != null)
            {
                string userName = loginUser.UserName;
                _tokenService.DelLoginUser(loginUser.Token);
                _ = Task.Factory.StartNew(async () =>
                {
                    await _sysLogininforService.AddAsync(userName, Constants.LOGOUT, "退出成功");
                });
            }

            return AjaxResult.Success("退出成功");
        }

        [HttpGet("/getInfo")]
        public async Task<AjaxResult> GetInfo()
        {
            SysUserDto user = SecurityUtils.GetLoginUser().User;
            List<string> roles = await _sysPermissionService.GetRolePermissionAsync(user);
            List<string> permissions = _sysPermissionService.GetMenuPermission(user);
            AjaxResult ajax = AjaxResult.Success();
            ajax.Add("user", user);
            ajax.Add("roles", roles);
            ajax.Add("permissions", permissions);
            return ajax;
        }

        [HttpGet("/getRouters")]
        public AjaxResult GetRouters()
        {
            long userId = SecurityUtils.GetUserId();
            List<SysMenu> menus = _sysMenuService.SelectMenuTreeByUserId(userId);
            var treeMenus = _sysMenuService.BuildMenus(menus);
            return AjaxResult.Success(treeMenus);
        }
    }
}