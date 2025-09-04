using RuoYi.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Dtos
{
    public class SysDictTypeDto : BaseDto
    {
        [Excel(Name = "字典主键")]
        public long? DictId { get; set; }

        [Excel(Name = "字典名称")]
        public string? DictName { get; set; }

        [Excel(Name = "字典类型")]
        [RegularExpression(@"^[a-z][a-z0-9_]*$", ErrorMessage = "字典类型必须以字母开头，且只能为（小写字母，数字，下滑线）")]
        public string? DictType { get; set; }
        public string? Status { get; set; }

        [Excel(Name = "状态")]
        public string? StatusDesc { get; set; }
    }
}