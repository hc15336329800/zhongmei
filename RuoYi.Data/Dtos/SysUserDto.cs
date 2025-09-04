using Newtonsoft.Json;
using RuoYi.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Dtos
{
    public class SysUserDto : BaseDto
    {
        [Excel(Name = "用户序号")]
        public long? UserId { get; set; }

        [Excel(Name = "部门编号", Type = ExcelOperationType.Import)]
        public long? DeptId { get; set; }

        [Excel(Name = "登录名称")]
        [Required(ErrorMessage = "用户账号不能为空"), MaxLength(30, ErrorMessage = "用户账号长度不能超过30个字符")]
        public string? UserName { get; set; }

        [Excel(Name = "用户名称")]
        [StringLength(30, ErrorMessage = "用户昵称长度不能超过30个字符")]
        public string? NickName { get; set; }

        [Excel(Name = "用户邮箱")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确"), MaxLength(50, ErrorMessage = "邮箱长度不能超过50个字符")]
        public string? Email { get; set; }

        [Excel(Name = "手机号码")]
        [StringLength(11, ErrorMessage = "手机号码长度不能超过11个字符")]
        public string? Phonenumber { get; set; }
        public string? Sex { get; set; }

        [Excel(Name = "用户性别")]
        public string? SexDesc { get; set; }
        public string? Avatar { get; set; }
        public string? Password { get; set; }
        public string? Status { get; set; }

        [Excel(Name = "帐号状态")]
        public string? StatusDesc { get; set; }
        public string? DelFlag { get; set; }

        [Excel(Name = "最后登录IP", Type = ExcelOperationType.Export)]
        public string? LoginIp { get; set; }

        [Excel(Name = "最后登录时间", Format = "yyyy-MM-dd HH:mm:ss", Type = ExcelOperationType.Export)]
        public DateTime LoginDate { get; set; }
        public SysDeptDto? Dept { get; set; }

        [Excel(Name = "部门名称", Type = ExcelOperationType.Export)]
        public string? DeptName { get; set; }

        [Excel(Name = "部门负责人", Type = ExcelOperationType.Export)]
        public string? DeptLeader { get; set; }
        public List<SysRoleDto>? Roles { get; set; }
        public List<long>? RoleIds { get; set; }
        public List<long>? PostIds { get; set; }
        public long? RoleId { get; set; }
        public bool? IsAllocated { get; set; }

        public bool ShouldSerializePassword()
        {
            return false;
        }
    }
}