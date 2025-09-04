namespace RuoYi.Data.Models;
public class SysCache
{
    public string? CacheName { get; set; } = string.Empty;
    public string? CacheKey { get; set; } = string.Empty;
    public string? CacheValue { get; set; } = string.Empty;
    public string? Remark { get; set; } = string.Empty;

    public SysCache()
    {
    }

    public SysCache(string cacheName, string remark)
    {
        this.CacheName = cacheName;
        this.Remark = remark;
    }

    public SysCache(string cacheName, string cacheKey, string cacheValue)
    {
        this.CacheName = cacheName;
        this.CacheKey = cacheKey;
        this.CacheValue = cacheValue;
    }
}