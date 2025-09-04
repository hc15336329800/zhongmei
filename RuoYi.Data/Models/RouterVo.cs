namespace RuoYi.Data.Models
{
    public class RouterVo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Hidden { get; set; }
        public string Redirect { get; set; }
        public string Component { get; set; }
        public string Query { get; set; }
        public bool AlwaysShow { get; set; }
        public MetaVo Meta { get; set; }
        public List<RouterVo> Children { get; set; }
    }
}