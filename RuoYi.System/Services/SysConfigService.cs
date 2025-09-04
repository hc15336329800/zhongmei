using RuoYi.Common.Constants;
using RuoYi.Framework.Cache;
using RuoYi.Framework.Exceptions;
using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysConfigService : BaseService<SysConfig, SysConfigDto>, ITransient
{
    private readonly ILogger<SysConfigService> _logger;
    private readonly ICache _cache;
    private readonly SysConfigRepository _sysConfigRepository;
    public SysConfigService(ILogger<SysConfigService> logger, ICache cache, SysConfigRepository sysConfigRepository)
    {
        _logger = logger;
        _cache = cache;
        _sysConfigRepository = sysConfigRepository;
        BaseRepo = sysConfigRepository;
    }

    public async Task<SysConfig> GetAsync(int id)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.ConfigId == id);
        return entity;
    }

    public string SelectConfigByKey(string configKey)
    {
        string? configValue = _cache.GetString(GetCacheKey(configKey));
        if (!string.IsNullOrEmpty(configValue))
        {
            return configValue;
        }

        SysConfig config = _sysConfigRepository.FirstOrDefault(e => e.ConfigKey == configKey);
        if (config != null)
        {
            _cache.SetString(GetCacheKey(configKey), config.ConfigValue ?? "");
            return config.ConfigValue ?? "";
        }

        return string.Empty;
    }

    private string GetCacheKey(string configKey)
    {
        return CacheConstants.SYS_CONFIG_KEY + configKey;
    }

    public bool IsCaptchaEnabled()
    {
        string captchaEnabled = SelectConfigByKey("sys.account.captchaEnabled");
        if (string.IsNullOrEmpty(captchaEnabled))
        {
            return true;
        }

        return Convert.ToBoolean(captchaEnabled);
    }

    public async Task<bool> InsertConfigAsync(SysConfigDto config)
    {
        bool success = await _sysConfigRepository.InsertAsync(config);
        if (success)
        {
            await _cache.SetStringAsync(GetCacheKey(config.ConfigKey!), config.ConfigValue!);
        }

        return success;
    }

    public async Task<int> UpdateConfigAsync(SysConfigDto config)
    {
        SysConfig temp = await this.GetAsync(config.ConfigId ?? 0);
        if (!temp.ConfigKey!.Equals(config.ConfigKey))
        {
            _cache.Remove(GetCacheKey(temp.ConfigKey));
        }

        int row = await _sysConfigRepository.UpdateAsync(config);
        if (row > 0)
        {
            await _cache.SetStringAsync(GetCacheKey(config.ConfigKey!), config.ConfigValue!);
        }

        return row;
    }

    public async Task DeleteConfigByIdsAsync(int[] configIds)
    {
        foreach (int configId in configIds)
        {
            SysConfig config = await this.GetAsync(configId);
            if (StringUtils.Equals(UserConstants.YES, config.ConfigType))
            {
                throw new ServiceException($"内置参数【{config.ConfigKey}】不能删除 ");
            }

            await _sysConfigRepository.DeleteAsync(configId);
            await _cache.RemoveAsync(GetCacheKey(config.ConfigKey!));
        }
    }

    public void ResetConfigCache()
    {
        ClearConfigCache();
        LoadingConfigCache();
    }

    public void ClearConfigCache()
    {
        _cache.RemoveByPattern(CacheConstants.SYS_CONFIG_KEY + "*");
    }

    public void LoadingConfigCache()
    {
        List<SysConfig> configsList = _sysConfigRepository.GetList(new SysConfigDto());
        foreach (SysConfig config in configsList)
        {
            _cache.SetString(GetCacheKey(config.ConfigKey!), config.ConfigValue!);
        }
    }

    public bool CheckConfigKeyUnique(SysConfigDto config)
    {
        int configId = config.ConfigId ?? 0;
        SysConfig info = _sysConfigRepository.CheckConfigKeyUnique(config.ConfigKey!);
        if (info != null && info.ConfigId != configId)
        {
            return UserConstants.NOT_UNIQUE;
        }

        return UserConstants.UNIQUE;
    }
}