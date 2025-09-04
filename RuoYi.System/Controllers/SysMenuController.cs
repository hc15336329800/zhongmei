using RuoYi.Common.Constants;
using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.System.Services;

namespace RuoYi.System.Controllers
{
    [ApiDescriptionSettings("System")]
    [Route("system/menu")]
    public class SysMenuController : ControllerBase
    {
        private readonly ILogger<SysMenuController> _logger;
        private readonly SysMenuService _sysMenuService;
        public SysMenuController(ILogger<SysMenuController> logger, SysMenuService sysMenuService)
        {
            _logger = logger;
            _sysMenuService = sysMenuService;
        }

        [HttpGet("list")]
        [AppAuthorize("system:menu:list")]
        public async Task<AjaxResult> SysMenuListAsync([FromQuery] SysMenuDto dto)
        {
            var data = await _sysMenuService.SelectMenuListAsync(dto, SecurityUtils.GetUserId());
            return AjaxResult.Success(data);
        }

        [HttpGet("{menuId}")]
        [AppAuthorize("system:menu:query")]
        public async Task<AjaxResult> Get(long? menuId)
        {
            var data = await _sysMenuService.GetAsync(menuId);
            return AjaxResult.Success(data);
        }

        [HttpGet("treeselect")]
        public async Task<AjaxResult> Treeselect([FromQuery] SysMenuDto dto)
        {
            var menus = await _sysMenuService.SelectMenuListAsync(dto, SecurityUtils.GetUserId());
            var data = _sysMenuService.BuildMenuTreeSelect(menus);
            return AjaxResult.Success(data);
        }

        [HttpGet("roleMenuTreeselect/{roleId}")]
        public async Task<AjaxResult> RoleMenuTreeselectAsync(long roleId)
        {
            List<SysMenu> menus = await _sysMenuService.SelectMenuListAsync(SecurityUtils.GetUserId());
            var ajax = AjaxResult.Success();
            ajax.Add("checkedKeys", _sysMenuService.SelectMenuListByRoleId(roleId));
            ajax.Add("menus", _sysMenuService.BuildMenuTreeSelect(menus));
            return ajax;
        }

        [HttpPost("")]
        [AppAuthorize("system:menu:add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [RuoYi.System.Log(Title = "菜单管理", BusinessType = BusinessType.INSERT)]
        public async Task<AjaxResult> Add([FromBody] SysMenuDto menu)
        {
            if (!_sysMenuService.CheckMenuNameUnique(menu))
            {
                return AjaxResult.Error("新增菜单'" + menu.MenuName + "'失败，菜单名称已存在");
            }
            else if (UserConstants.YES_FRAME.Equals(menu.IsFrame) && !StringUtils.IsHttp(menu.Path))
            {
                return AjaxResult.Error("新增菜单'" + menu.MenuName + "'失败，地址必须以http(s)://开头");
            }

            var data = await _sysMenuService.InsertAsync(menu);
            return AjaxResult.Success(data);
        }

        [HttpPut("")]
        [AppAuthorize("system:menu:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [RuoYi.System.Log(Title = "菜单管理", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] SysMenuDto menu)
        {
            if (!_sysMenuService.CheckMenuNameUnique(menu))
            {
                return AjaxResult.Error("修改菜单'" + menu.MenuName + "'失败，菜单名称已存在");
            }
            else if (UserConstants.YES_FRAME.Equals(menu.IsFrame) && !StringUtils.IsHttp(menu.Path))
            {
                return AjaxResult.Error("修改菜单'" + menu.MenuName + "'失败，地址必须以http(s)://开头");
            }
            else if (menu.MenuId.Equals(menu.ParentId))
            {
                return AjaxResult.Error("修改菜单'" + menu.MenuName + "'失败，上级菜单不能选择自己");
            }

            var data = await _sysMenuService.UpdateAsync(menu);
            return AjaxResult.Success(data);
        }

        [HttpDelete("{menuId}")]
        [AppAuthorize("system:menu:remove")]
        [RuoYi.System.Log(Title = "菜单管理", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(long menuId)
        {
            if (_sysMenuService.HasChildByMenuId(menuId))
            {
                return AjaxResult.Error("存在子菜单,不允许删除");
            }

            if (_sysMenuService.CheckMenuExistRole(menuId))
            {
                return AjaxResult.Error("菜单已分配,不允许删除");
            }

            var data = await _sysMenuService.DeleteAsync(menuId);
            return AjaxResult.Success(data);
        }
    }
}