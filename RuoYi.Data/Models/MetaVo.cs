namespace RuoYi.Data.Models
{
    public class MetaVo
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool NoCache { get; set; }
        public string Link { get; set; }

        public MetaVo()
        {
        }

        public MetaVo(string title, string icon)
        {
            this.Title = title;
            this.Icon = icon;
        }

        public MetaVo(string title, string icon, bool noCache)
        {
            this.Title = title;
            this.Icon = icon;
            this.NoCache = noCache;
        }

        public MetaVo(string title, string icon, string link)
        {
            this.Title = title;
            this.Icon = icon;
            this.Link = link;
        }

        public MetaVo(string title, string icon, bool noCache, string link)
        {
            this.Title = title;
            this.Icon = icon;
            this.NoCache = noCache;
            if (!string.IsNullOrEmpty(link) && link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                this.Link = link;
            }
        }
    }
}