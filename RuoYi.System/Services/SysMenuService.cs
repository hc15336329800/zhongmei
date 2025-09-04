using RuoYi.Common.Constants;
using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysMenuService : BaseService<SysMenu, SysMenuDto>, ITransient
{
    private readonly ILogger<SysMenuService> _logger;
    private readonly SysMenuRepository _sysMenuRepository;
    private readonly SysRoleRepository _sysRoleRepository;
    private readonly SysRoleMenuRepository _sysRoleMenuRepository;
    public SysMenuService(ILogger<SysMenuService> logger, SysMenuRepository sysMenuRepository, SysRoleRepository sysRoleRepository, SysRoleMenuRepository sysRoleMenuRepository)
    {
        BaseRepo = sysMenuRepository;
        _logger = logger;
        _sysMenuRepository = sysMenuRepository;
        _sysRoleRepository = sysRoleRepository;
        _sysRoleMenuRepository = sysRoleMenuRepository;
    }

    public async Task<List<SysMenu>> SelectMenuListAsync(long userId)
    {
        return await SelectMenuListAsync(new SysMenuDto(), userId);
    }

    public async Task<List<SysMenu>> SelectMenuListAsync(SysMenuDto menu, long userId)
    {
        List<SysMenu> menuList = null;
        if (SecurityUtils.IsAdmin(userId))
        {
            menuList = await _sysMenuRepository.SelectMenuListAsync(menu);
        }
        else
        {
            menu.UserId = userId;
            menuList = await _sysMenuRepository.SelectMenuListByUserIdAsync(menu);
        }

        return menuList;
    }

    public async Task<SysMenu> GetAsync(long? id)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.MenuId == id);
        return entity;
    }

    public List<string> SelectMenuPermsByUserId(long userId)
    {
        List<string> perms = _sysMenuRepository.SelectMenuPermsByUserId(userId);
        List<string> permsSet = new List<string>();
        foreach (string perm in perms)
        {
            if (!string.IsNullOrEmpty(perm))
            {
                permsSet.AddRange(perm.Trim().Split(","));
            }
        }

        return permsSet;
    }

    public List<string> SelectMenuPermsByRoleId(long roleId)
    {
        List<string> perms = _sysMenuRepository.SelectMenuPermsByRoleId(roleId);
        List<string> permsSet = new List<string>();
        foreach (string perm in perms)
        {
            if (!string.IsNullOrEmpty(perm))
            {
                permsSet.AddRange(perm.Trim().Split(","));
            }
        }

        return permsSet;
    }

    public List<SysMenu> SelectMenuTreeByUserId(long userId)
    {
        List<SysMenu> menus = null;
        if (SecurityUtils.IsAdmin(userId))
        {
            menus = _sysMenuRepository.SelectMenuTreeAll();
        }
        else
        {
            menus = _sysMenuRepository.SelectMenuTreeByUserId(userId);
        }

        return GetChildPerms(menus, 0);
    }

    public List<long> SelectMenuListByRoleId(long roleId)
    {
        SysRole role = _sysRoleRepository.GetRoleById(roleId);
        return _sysMenuRepository.SelectMenuListByRoleId(roleId, role.MenuCheckStrictly);
    }

    public List<RouterVo> BuildMenus(List<SysMenu> menus)
    {
        List<RouterVo> routers = new List<RouterVo>();
        foreach (SysMenu menu in menus)
        {
            RouterVo router = new RouterVo();
            router.Hidden = Status.Disabled.Equals(menu.Visible);
            router.Name = GetRouteName(menu);
            router.Path = GetRouterPath(menu);
            router.Component = GetComponent(menu);
            router.Query = menu.Query;
            router.Meta = new MetaVo(menu.MenuName, menu.Icon, Status.Disabled.Equals(menu.IsCache), menu.Path);
            List<SysMenu> cMenus = menu.Children;
            if (cMenus != null && UserConstants.TYPE_DIR.Equals(menu.MenuType))
            {
                router.AlwaysShow = true;
                router.Redirect = "noRedirect";
                router.Children = BuildMenus(cMenus);
            }
            else if (IsMenuFrame(menu))
            {
                router.Meta = null;
                List<RouterVo> childrenList = new List<RouterVo>();
                RouterVo children = new RouterVo();
                children.Path = menu.Path;
                children.Component = menu.Component;
                children.Name = menu.Path.ToUpperCamelCase();
                children.Meta = new MetaVo(menu.MenuName, menu.Icon, Status.Disabled.Equals(menu.IsCache), menu.Path);
                children.Query = menu.Query;
                childrenList.Add(children);
                router.Children = childrenList;
            }
            else if (menu.ParentId == 0 && IsInnerLink(menu))
            {
                router.Meta = new MetaVo(menu.MenuName, menu.Icon);
                router.Path = "/";
                List<RouterVo> childrenList = new List<RouterVo>();
                RouterVo children = new RouterVo();
                string routerPath = InnerLinkReplaceEach(menu.Path);
                children.Path = routerPath;
                children.Component = UserConstants.INNER_LINK;
                children.Name = routerPath.ToUpperCamelCase();
                children.Meta = new MetaVo(menu.MenuName, menu.Icon, menu.Path);
                childrenList.Add(children);
                router.Children = childrenList;
            }

            routers.Add(router);
        }

        return routers;
    }

    public List<SysMenu> BuildMenuTree(List<SysMenu> menus)
    {
        List<SysMenu> returnList = new List<SysMenu>();
        List<long> tempList = menus.Select(m => m.MenuId).ToList();
        foreach (SysMenu menu in menus)
        {
            if (!tempList.Contains(menu.ParentId))
            {
                RecursionFn(menus, menu);
                returnList.Add(menu);
            }
        }

        if (returnList.IsEmpty())
        {
            returnList = menus;
        }

        return returnList;
    }

    public List<TreeSelect> BuildMenuTreeSelect(List<SysMenu> menus)
    {
        List<SysMenu> menuTrees = BuildMenuTree(menus);
        return menuTrees.Select(m => new TreeSelect(m)).ToList();
    }

    public SysMenu SelectMenuById(long menuId)
    {
        return _sysMenuRepository.SelectMenuById(menuId);
    }

    public bool HasChildByMenuId(long menuId)
    {
        return _sysMenuRepository.HasChildByMenuId(menuId);
    }

    public bool CheckMenuExistRole(long menuId)
    {
        return _sysRoleMenuRepository.CheckMenuExistRole(menuId);
    }

    public bool CheckMenuNameUnique(SysMenuDto menu)
    {
        long menuId = menu.MenuId;
        SysMenu info = _sysMenuRepository.CheckMenuNameUnique(menu.MenuName!, menu.ParentId);
        if (info != null && info.MenuId != menuId)
        {
            return UserConstants.NOT_UNIQUE;
        }

        return UserConstants.UNIQUE;
    }

    public string GetRouteName(SysMenu menu)
    {
        string routerName = menu.Path.ToUpperCamelCase();
        if (IsMenuFrame(menu))
        {
            routerName = string.Empty;
        }

        return routerName;
    }

    public string GetRouterPath(SysMenu menu)
    {
        string routerPath = menu.Path;
        if (menu.ParentId != 0 && IsInnerLink(menu))
        {
            routerPath = InnerLinkReplaceEach(routerPath);
        }

        if (0 == menu.ParentId && UserConstants.TYPE_DIR.Equals(menu.MenuType) && UserConstants.NO_FRAME.Equals(menu.IsFrame))
        {
            routerPath = "/" + menu.Path;
        }
        else if (IsMenuFrame(menu))
        {
            routerPath = "/";
        }

        return routerPath;
    }

    public string GetComponent(SysMenu menu)
    {
        string component = UserConstants.LAYOUT;
        if (StringUtils.IsNotEmpty(menu.Component) && !IsMenuFrame(menu))
        {
            component = menu.Component;
        }
        else if (StringUtils.IsEmpty(menu.Component) && menu.ParentId != 0 && IsInnerLink(menu))
        {
            component = UserConstants.INNER_LINK;
        }
        else if (StringUtils.IsEmpty(menu.Component) && IsParentView(menu))
        {
            component = UserConstants.PARENT_VIEW;
        }

        return component;
    }

    public bool IsMenuFrame(SysMenu menu)
    {
        return menu.ParentId == 0 && UserConstants.TYPE_MENU.Equals(menu.MenuType) && menu.IsFrame.Equals(UserConstants.NO_FRAME);
    }

    public bool IsInnerLink(SysMenu menu)
    {
        return menu.IsFrame.Equals(UserConstants.NO_FRAME) && StringUtils.IsHttp(menu.Path);
    }

    public bool IsParentView(SysMenu menu)
    {
        return menu.ParentId != 0 && UserConstants.TYPE_DIR.Equals(menu.MenuType);
    }

    public List<SysMenu> GetChildPerms(List<SysMenu> list, int parentId)
    {
        List<SysMenu> returnList = new List<SysMenu>();
        foreach (SysMenu t in list)
        {
            if (t.ParentId == parentId)
            {
                RecursionFn(list, t);
                returnList.Add(t);
            }
        }

        return returnList;
    }

    private void RecursionFn(List<SysMenu> list, SysMenu t)
    {
        List<SysMenu> childList = GetChildList(list, t);
        t.Children = childList;
        foreach (SysMenu tChild in childList)
        {
            if (HasChild(list, tChild))
            {
                RecursionFn(list, tChild);
            }
        }
    }

    private List<SysMenu> GetChildList(List<SysMenu> list, SysMenu t)
    {
        List<SysMenu> tList = new List<SysMenu>();
        foreach (SysMenu n in list)
        {
            if (n.ParentId == t.MenuId)
            {
                tList.Add(n);
            }
        }

        return tList;
    }

    private bool HasChild(List<SysMenu> list, SysMenu t)
    {
        return GetChildList(list, t).Count > 0;
    }

    public string InnerLinkReplaceEach(string path)
    {
        var searchList = new string[]
        {
            Constants.HTTP,
            Constants.HTTPS,
            Constants.WWW,
            "."
        };
        var replacementList = new string[]
        {
            "",
            "",
            "",
            "/"
        };
        for (var i = 0; i < searchList.Length; i++)
        {
            path.Replace(searchList[i], replacementList[i]);
        }

        return path;
    }
}