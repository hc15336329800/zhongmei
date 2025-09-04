using RuoYi.Data.Dtos;

namespace RuoYi.Data.Models
{
    public class LoginUser
    {
        public LoginUser()
        {
        }

        public LoginUser(long userId, long deptId, SysUserDto user, List<string> permissions)
        {
            this.UserId = userId;
            this.DeptId = deptId;
            this.User = user;
            this.Permissions = permissions;
        }

        [Newtonsoft.Json.JsonProperty(Order = 0)]
        public string UserName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public long UserId { get; set; }
        public long DeptId { get; set; }
        public string Token { get; set; }
        public long LoginTime { get; set; }
        public long ExpireTime { get; set; }
        public string IpAddr { get; set; }
        public string LoginLocation { get; set; }
        public string Browser { get; set; }
        public string OS { get; set; }
        public SysUserDto User { get; set; }
        public List<string> Permissions { get; set; }
    }
}