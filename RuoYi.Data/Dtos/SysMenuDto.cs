using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Dtos
{
    public class SysMenuDto : BaseDto
    {
        public long MenuId { get; set; }

        [Required(ErrorMessage = "菜单名称不能为空"), MaxLength(50, ErrorMessage = "菜单名称长度不能超过50个字符")]
        public string? MenuName { get; set; }
        public string? ParentName { get; set; }
        public long ParentId { get; set; }

        [Required(ErrorMessage = "显示顺序不能为空")]
        public int? OrderNum { get; set; }

        [MaxLength(200, ErrorMessage = "路由地址不能超过200个字符")]
        public string? Path { get; set; }

        [MaxLength(255, ErrorMessage = "组件路径不能超过255个字符")]
        public string? Component { get; set; }
        public string? Query { get; set; }
        public string? IsFrame { get; set; }
        public string? IsCache { get; set; }

        [Required(ErrorMessage = "菜单类型不能为空")]
        public string? MenuType { get; set; }
        public string? Visible { get; set; }
        public string? Status { get; set; }

        [MaxLength(100, ErrorMessage = "权限标识长度不能超过100个字符")]
        public string? Perms { get; set; }
        public string? Icon { get; set; }

        public List<SysMenuDto> Children = new List<SysMenuDto>();
        public string? RoleStatus { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public List<string> MenuTypes { get; set; } = new List<string>();
    }
}