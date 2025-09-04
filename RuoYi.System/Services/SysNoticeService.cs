using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysNoticeService : BaseService<SysNotice, SysNoticeDto>, ITransient
{
    private readonly ILogger<SysNoticeService> _logger;
    private readonly SysNoticeRepository _sysNoticeRepository;
    public SysNoticeService(ILogger<SysNoticeService> logger, SysNoticeRepository sysNoticeRepository)
    {
        BaseRepo = sysNoticeRepository;
        _logger = logger;
        _sysNoticeRepository = sysNoticeRepository;
    }

    public async Task<SysNotice> GetAsync(int id)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.NoticeId == id);
        return entity;
    }

    public async Task<SysNoticeDto> GetDtoAsync(int id)
    {
        var dto = new SysNoticeDto
        {
            NoticeId = id
        };
        return await _sysNoticeRepository.GetDtoFirstAsync(dto);
    }
}