using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_role_dept", "角色和部门关联表")]
    public class SysRoleDept : BaseEntity
    {
        [SugarColumn(ColumnName = "role_id", ColumnDescription = "角色ID")]
        public long RoleId { get; set; }

        [SugarColumn(ColumnName = "dept_id", ColumnDescription = "部门ID")]
        public long DeptId { get; set; }
    }
}