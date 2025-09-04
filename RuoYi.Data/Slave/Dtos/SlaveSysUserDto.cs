using RuoYi.Data.Dtos;

namespace RuoYi.Data.Slave.Dtos
{
    public class SlaveSysUserDto : BaseDto
    {
        public long UserId { get; set; }
        public long DeptId { get; set; }
        public string? UserName { get; set; }
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Sex { get; set; }
        public string? Avatar { get; set; }
        public string? Password { get; set; }
        public string? Status { get; set; }
        public string? DelFlag { get; set; }
        public string? LoginIp { get; set; }
        public DateTime LoginDate { get; set; }
        public SysDeptDto Dept { get; set; }
        public List<SysRoleDto> Roles { get; set; }
        public long[] RoleIds { get; set; }
        public long[] PostIds { get; set; }
        public long RoleId { get; set; }

        public bool ShouldSerializePassword()
        {
            return false;
        }
    }
}