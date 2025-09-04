using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_user_role", "用户和角色关联表")]
    public class SysUserRole : BaseEntity
    {
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID")]
        public long UserId { get; set; }

        [SugarColumn(ColumnName = "role_id", ColumnDescription = "角色ID")]
        public long RoleId { get; set; }
    }
}