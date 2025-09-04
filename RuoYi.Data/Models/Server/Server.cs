namespace RuoYi.Data.Models
{
    public class Server
    {
        public Cpu Cpu { get; set; } = new Cpu();
        public Mem Mem { get; set; } = new Mem();
        public Clr Clr { get; set; } = new Clr();
        public Sys Sys { get; set; } = new Sys();
        public List<SysFile> SysFiles { get; set; } = new List<SysFile>();
    }
}