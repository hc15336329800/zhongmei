using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysOperLogService : BaseService<SysOperLog, SysOperLogDto>, ITransient
{
    private readonly ILogger<SysOperLogService> _logger;
    private readonly SysOperLogRepository _sysOperLogRepository;
    public SysOperLogService(ILogger<SysOperLogService> logger, SysOperLogRepository sysOperLogRepository)
    {
        BaseRepo = sysOperLogRepository;
        _logger = logger;
        _sysOperLogRepository = sysOperLogRepository;
    }

    public async Task<SysOperLog> GetAsync(long id)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.OperId == id);
        return entity;
    }

    public async Task<SysOperLogDto> GetDtoAsync(long id)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.OperId == id);
        var dto = entity.Adapt<SysOperLogDto>();
        return dto;
    }

    public void Clean()
    {
        _sysOperLogRepository.Truncate();
    }
}