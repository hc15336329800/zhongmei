using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_logininfor", "系统访问记录")]
    public class SysLogininfor : BaseEntity
    {
        [SugarColumn(ColumnName = "info_id", ColumnDescription = "访问ID", IsPrimaryKey = true, IsIdentity = true)]
        public long InfoId { get; set; }

        [SugarColumn(ColumnName = "user_name", ColumnDescription = "用户账号")]
        public string? UserName { get; set; }

        [SugarColumn(ColumnName = "ipaddr", ColumnDescription = "登录IP地址")]
        public string? Ipaddr { get; set; }

        [SugarColumn(ColumnName = "login_location", ColumnDescription = "登录地点")]
        public string? LoginLocation { get; set; }

        [SugarColumn(ColumnName = "browser", ColumnDescription = "浏览器类型")]
        public string? Browser { get; set; }

        [SugarColumn(ColumnName = "os", ColumnDescription = "操作系统")]
        public string? Os { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "登录状态（0成功 1失败）")]
        public string? Status { get; set; }

        [SugarColumn(ColumnName = "msg", ColumnDescription = "提示消息")]
        public string? Msg { get; set; }

        [SugarColumn(ColumnName = "login_time", ColumnDescription = "访问时间")]
        public DateTime? LoginTime { get; set; }
    }
}