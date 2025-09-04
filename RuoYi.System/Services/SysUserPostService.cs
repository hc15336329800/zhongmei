using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysUserPostService : BaseService<SysUserPost, SysUserPostDto>, ITransient
{
    private readonly ILogger<SysUserPostService> _logger;
    private readonly SysUserPostRepository _sysUserPostRepository;
    public SysUserPostService(ILogger<SysUserPostService> logger, SysUserPostRepository sysUserPostRepository)
    {
        _logger = logger;
        _sysUserPostRepository = sysUserPostRepository;
        BaseRepo = sysUserPostRepository;
    }

    public async Task<SysUserPostDto> GetAsync(long? id)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.UserId == id);
        var dto = entity.Adapt<SysUserPostDto>();
        return dto;
    }
}