namespace RuoYi.Data.Dtos
{
    public class SysUserRoleDto : BaseDto
    {
        public long UserId { get; set; }
        public List<long> UserIds { get; set; }
        public long RoleId { get; set; }
    }
}