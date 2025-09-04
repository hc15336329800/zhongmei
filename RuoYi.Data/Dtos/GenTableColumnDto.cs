namespace RuoYi.Data.Dtos
{
    public class GenTableColumnDto : BaseDto
    {
        public long ColumnId { get; set; }
        public long? TableId { get; set; }
        public string? ColumnName { get; set; }
        public string? ColumnComment { get; set; }
        public string? ColumnType { get; set; }
        public string? NetType { get; set; }
        public string? NetField { get; set; }
        public string? IsPk { get; set; }
        public string? IsIncrement { get; set; }
        public string? IsRequired { get; set; }
        public string? IsInsert { get; set; }
        public string? IsEdit { get; set; }
        public string? IsList { get; set; }
        public string? IsQuery { get; set; }
        public string? QueryType { get; set; }
        public string? HtmlType { get; set; }
        public string? DictType { get; set; }
        public int? Sort { get; set; }
    }
}