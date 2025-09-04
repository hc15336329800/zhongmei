namespace RuoYi.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ExcelAttribute : Attribute
    {
        public ExcelOperationType Type { get; set; }
        public string? Name { get; set; }
        public string[]? Aliases { get; set; }
        public double Width { get; set; }
        public string? Format { get; set; }
        public bool Ignore { get; set; }
        public int? Index { get; set; }
    }

    public enum ExcelOperationType
    {
        All,
        Export,
        Import
    }
}