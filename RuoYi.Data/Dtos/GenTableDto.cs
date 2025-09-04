using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Dtos
{
    public class GenTableDto : BaseDto
    {
        public long TableId { get; set; }

        [Required(ErrorMessage = "表名称不能为空")]
        public string? TableName { get; set; }

        [Required(ErrorMessage = "表描述不能为空")]
        public string? TableComment { get; set; }
        public string? SubTableName { get; set; }
        public string? SubTableFkName { get; set; }

        [Required(ErrorMessage = "实体类名称不能为空")]
        public string? ClassName { get; set; }
        public string? TplCategory { get; set; }

        [Required(ErrorMessage = "生成包路径不能为空")]
        public string? PackageName { get; set; }

        [Required(ErrorMessage = "生成模块名不能为空")]
        public string? ModuleName { get; set; }

        [Required(ErrorMessage = "生成业务名不能为空")]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "生成功能名不能为空")]
        public string? FunctionName { get; set; }

        [Required(ErrorMessage = "作者不能为空")]
        public string? FunctionAuthor { get; set; }
        public string? GenType { get; set; }
        public string? GenPath { get; set; }
        public string? Options { get; set; }
        public string? TreeCode { get; set; }
        public string? TreeParentCode { get; set; }
        public string? TreeName { get; set; }
        public string? ParentMenuId { get; set; }
        public string? ParentMenuName { get; set; }
        public GenTableColumnDto? PkColumn { get; set; }
        public GenTableDto? SubTable { get; set; }
        public List<GenTableColumnDto>? Columns { get; set; }
    }

    public class GenTableOptions
    {
        public string? TreeCode { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string? TreeParentCode { get; set; }
        public string? TreeName { get; set; }
        public string? ParentMenuId { get; set; }
        public string? ParentMenuName { get; set; }
    }
}