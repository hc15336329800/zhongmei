using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysRoleMenuService : BaseService<SysRoleMenu, SysRoleMenuDto>, ITransient
{
    private readonly ILogger<SysRoleMenuService> _logger;
    private readonly SysRoleMenuRepository _sysRoleMenuRepository;
    public SysRoleMenuService(ILogger<SysRoleMenuService> logger, SysRoleMenuRepository sysRoleMenuRepository)
    {
        _logger = logger;
        _sysRoleMenuRepository = sysRoleMenuRepository;
        BaseRepo = sysRoleMenuRepository;
    }

    public async Task<SysRoleMenuDto> GetAsync(long? id)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.RoleId == id);
        var dto = entity.Adapt<SysRoleMenuDto>();
        return dto;
    }
}