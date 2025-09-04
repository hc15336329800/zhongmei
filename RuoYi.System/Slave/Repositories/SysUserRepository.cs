using RuoYi.Data.Slave.Dtos;
using RuoYi.Data.Slave.Entities;

namespace RuoYi.System.Slave.Repositories;
public class SysUserRepository : BaseRepository<SlaveSysUser, SlaveSysUserDto>
{
    public SysUserRepository(ISqlSugarRepository<SlaveSysUser> sqlSugarRepository)
    {
        Repo = sqlSugarRepository;
    }

    public override ISugarQueryable<SlaveSysUser> Queryable(SlaveSysUserDto dto)
    {
        return Repo.AsQueryable();
    }

    public override ISugarQueryable<SlaveSysUserDto> DtoQueryable(SlaveSysUserDto dto)
    {
        return Repo.AsQueryable().Select((t) => new SlaveSysUserDto { CreateBy = t.CreateBy, CreateTime = t.CreateTime, UpdateBy = t.UpdateBy, UpdateTime = t.UpdateTime, });
    }
}