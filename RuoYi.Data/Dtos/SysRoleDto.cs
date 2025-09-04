using RuoYi.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Dtos
{
    public class SysRoleDto : BaseDto
    {
        [Excel(Name = "角色序号")]
        public long RoleId { get; set; }

        [Excel(Name = "角色名称")]
        [Required(ErrorMessage = "角色名称不能为空"), MaxLength(30, ErrorMessage = "角色名称不能超过30个字符")]
        public string? RoleName { get; set; }

        [Excel(Name = "角色权限")]
        [Required(ErrorMessage = "角色权限不能为空"), MaxLength(100, ErrorMessage = "角色权限不能超过100个字符")]
        public string? RoleKey { get; set; }

        [Excel(Name = "角色排序")]
        [Required(ErrorMessage = "显示顺序不能为空")]
        public int? RoleSort { get; set; }
        public string? DataScope { get; set; }

        [Excel(Name = "数据范围")]
        public string? DataScopeDesc { get; set; }
        public bool? MenuCheckStrictly { get; set; }
        public bool? DeptCheckStrictly { get; set; }
        public string? Status { get; set; }

        [Excel(Name = "角色状态")]
        public string? StatusDesc { get; set; }
        public string? DelFlag { get; set; }
        public bool? Flag { get; set; } = false;
        public long[]? MenuIds { get; set; }
        public long[]? DeptIds { get; set; }
        public List<string>? Permissions { get; set; }
        public long? UserId { get; set; }
        public string? UserName { get; set; }
    }
}