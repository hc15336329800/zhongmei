using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysRoleDeptService : BaseService<SysRoleDept, SysRoleDeptDto>, ITransient
{
    private readonly ILogger<SysRoleDeptService> _logger;
    private readonly SysRoleDeptRepository _sysRoleDeptRepository;
    public SysRoleDeptService(ILogger<SysRoleDeptService> logger, SysRoleDeptRepository sysRoleDeptRepository)
    {
        _logger = logger;
        _sysRoleDeptRepository = sysRoleDeptRepository;
        BaseRepo = sysRoleDeptRepository;
    }

    public async Task<SysRoleDeptDto> GetAsync(long? id)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.RoleId == id);
        var dto = entity.Adapt<SysRoleDeptDto>();
        return dto;
    }
}