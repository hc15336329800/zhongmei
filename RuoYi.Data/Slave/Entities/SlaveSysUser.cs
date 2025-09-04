using RuoYi.Data.Entities;
using SqlSugar;

namespace RuoYi.Data.Slave.Entities
{
    [Tenant(DataConstants.Slave)]
    [SugarTable("sys_user", "用户表")]
    public class SlaveSysUser : UserBaseEntity
    {
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID")]
        public long UserId { get; set; }

        [SugarColumn(ColumnName = "dept_id", ColumnDescription = "部门编号")]
        public long DeptId { get; set; }

        [SugarColumn(ColumnName = "user_name", ColumnDescription = "登录名称")]
        public string? UserName { get; set; }

        [SugarColumn(ColumnName = "nick_name", ColumnDescription = "用户名称")]
        public string? NickName { get; set; }

        [SugarColumn(ColumnName = "email", ColumnDescription = "用户邮箱")]
        public string? Email { get; set; }

        [SugarColumn(ColumnName = "phonenumber", ColumnDescription = "手机号码")]
        public string? PhoneNumber { get; set; }

        [SugarColumn(ColumnName = "sex", ColumnDescription = "用户性别")]
        public string? Sex { get; set; }

        [SugarColumn(ColumnName = "avatar", ColumnDescription = "用户头像")]
        public string? Avatar { get; set; }

        [SugarColumn(ColumnName = "password", ColumnDescription = "密码")]
        public string? Password { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "帐号状态")]
        public string? Status { get; set; }

        [SugarColumn(ColumnName = "del_flag", ColumnDescription = "删除标志")]
        public string? DelFlag { get; set; }

        [SugarColumn(ColumnName = "login_ip", ColumnDescription = "最后登录IP")]
        public string? LoginIp { get; set; }

        [SugarColumn(ColumnName = "login_date", ColumnDescription = "最后登录时间")]
        public DateTime LoginDate { get; set; }

        [SugarColumn(IsIgnore = true)]
        public SysDept Dept { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<SysRole> Roles { get; set; }

        [SugarColumn(IsIgnore = true)]
        public long[] RoleIds { get; set; }

        [SugarColumn(IsIgnore = true)]
        public long[] PostIds { get; set; }

        [SugarColumn(IsIgnore = true)]
        public long RoleId { get; set; }
    }
}