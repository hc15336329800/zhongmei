using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_menu", "菜单权限表")]
    public class SysMenu : UserBaseEntity
    {
        [SugarColumn(ColumnName = "menu_id", ColumnDescription = "菜单ID", IsPrimaryKey = true, IsIdentity = true)]
        public long MenuId { get; set; }

        [SugarColumn(ColumnName = "menu_name", ColumnDescription = "菜单名称")]
        public string? MenuName { get; set; }

        [SugarColumn(ColumnName = "parent_id", ColumnDescription = "父菜单ID")]
        public long ParentId { get; set; }

        [SugarColumn(ColumnName = "order_num", ColumnDescription = "显示顺序")]
        public int? OrderNum { get; set; }

        [SugarColumn(ColumnName = "path", ColumnDescription = "路由地址")]
        public string? Path { get; set; }

        [SugarColumn(ColumnName = "component", ColumnDescription = "组件路径")]
        public string? Component { get; set; }

        [SugarColumn(ColumnName = "query", ColumnDescription = "路由参数")]
        public string? Query { get; set; }

        [SugarColumn(ColumnName = "is_frame", ColumnDescription = "是否为外链（0是 1否）")]
        public string? IsFrame { get; set; }

        [SugarColumn(ColumnName = "is_cache", ColumnDescription = "是否缓存（0缓存 1不缓存）")]
        public string? IsCache { get; set; }

        [SugarColumn(ColumnName = "menu_type", ColumnDescription = "类型（M目录 C菜单 F按钮）")]
        public string? MenuType { get; set; }

        [SugarColumn(ColumnName = "visible", ColumnDescription = "显示状态（0显示 1隐藏）")]
        public string? Visible { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "菜单状态（0正常 1停用）")]
        public string? Status { get; set; }

        [SugarColumn(ColumnName = "perms", ColumnDescription = "权限字符串")]
        public string? Perms { get; set; }

        [SugarColumn(ColumnName = "icon", ColumnDescription = "菜单图标")]
        public string? Icon { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<SysMenu> Children { get; set; } = new List<SysMenu>();
    }
}