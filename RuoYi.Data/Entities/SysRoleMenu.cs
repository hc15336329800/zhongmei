using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_role_menu", "角色和菜单关联表")]
    public class SysRoleMenu : BaseEntity
    {
        [SugarColumn(ColumnName = "role_id", ColumnDescription = "角色ID")]
        public long RoleId { get; set; }

        [SugarColumn(ColumnName = "menu_id", ColumnDescription = "菜单ID")]
        public long MenuId { get; set; }
    }
}