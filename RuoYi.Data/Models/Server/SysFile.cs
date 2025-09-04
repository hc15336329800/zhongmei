namespace RuoYi.Data.Models
{
    public class SysFile
    {
        public string DirName { get; set; }
        public string SysTypeName { get; set; }
        public string TypeName { get; set; }
        public string Total { get; set; }
        public string Free { get; set; }
        public string Used { get; set; }
        public decimal Usage { get; set; }
    }
}