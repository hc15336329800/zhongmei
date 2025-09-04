using RuoYi.Data;
using RuoYi.Data.Slave.Dtos;
using RuoYi.Data.Slave.Entities;
using RuoYi.System.Slave.Repositories;

namespace RuoYi.System.Slave.Services
{
    public class SysUserService : BaseService<SlaveSysUser, SlaveSysUserDto>, ITransient
    {
        private readonly ILogger<SysUserService> _logger;
        private readonly SysUserRepository _sysUserRepository;
        public SysUserService(ILogger<SysUserService> logger, SysUserRepository sysUserRepository)
        {
            _logger = logger;
            _sysUserRepository = sysUserRepository;
            BaseRepo = sysUserRepository;
        }

        public async Task<SlaveSysUserDto> GetAsync(long? id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.UserId == id);
            var dto = entity.Adapt<SlaveSysUserDto>();
            return dto;
        }

        public async Task<SlaveSysUser> GetByUsernameAsync(string username)
        {
            return await base.FirstOrDefaultAsync(e => e.UserName == username);
        }
    }
}