using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Dtos
{
    public class SysDeptDto : BaseDto
    {
        public long? DeptId { get; set; }
        public long? ParentId { get; set; }
        public string? Ancestors { get; set; }

        [Required(ErrorMessage = "部门名称不能为空"), MaxLength(30, ErrorMessage = "部门名称不能超过30个字符")]
        public string? DeptName { get; set; }

        [Required(ErrorMessage = "显示顺序不能为空")]
        public int? OrderNum { get; set; }
        public string? Leader { get; set; }

        [MaxLength(30, ErrorMessage = "联系电话长度不能超过11个字符")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "邮箱格式不正确"), MaxLength(50, ErrorMessage = "邮箱长度不能超过50个字符")]
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? DelFlag { get; set; }
        public string? ParentName { get; set; }
        public List<SysDeptDto>? Children { get; set; }
        public bool? DeptCheckStrictly { get; set; }
        public long? RoleId { get; set; }
        public List<long>? ParentIds { get; set; }
    }
}