using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_user_post", "用户与岗位关联表")]
    public class SysUserPost : BaseEntity
    {
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID")]
        public long UserId { get; set; }

        [SugarColumn(ColumnName = "post_id", ColumnDescription = "岗位ID")]
        public long PostId { get; set; }
    }
}