using RuoYi.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;

namespace RuoYi.System.Repositories
{
    public class SysRoleDeptRepository : BaseRepository<SysRoleDept, SysRoleDeptDto>
    {
        public SysRoleDeptRepository(ISqlSugarRepository<SysRoleDept> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<SysRoleDept> Queryable(SysRoleDeptDto dto)
        {
            return Repo.AsQueryable().WhereIF(dto.RoleId > 0, (t) => t.RoleId == dto.RoleId).WhereIF(dto.DeptId > 0, (t) => t.DeptId == dto.DeptId);
        }

        public override ISugarQueryable<SysRoleDeptDto> DtoQueryable(SysRoleDeptDto dto)
        {
            return Repo.AsQueryable().WhereIF(dto.RoleId > 0, (t) => t.RoleId == dto.RoleId).WhereIF(dto.DeptId > 0, (t) => t.DeptId == dto.DeptId).Select((t) => new SysRoleDeptDto { RoleId = t.RoleId, DeptId = t.DeptId });
        }

        public async Task<int> DeleteByRoleIdAsync(long roleId)
        {
            return await base.DeleteAsync(rd => rd.RoleId == roleId);
        }

        public async Task<int> DeleteByRoleIdsAsync(List<long> roleIds)
        {
            return await base.DeleteAsync(rd => roleIds.Contains(rd.RoleId));
        }

        public async Task<int> DeleteByDeptIdAsync(long deptId)
        {
            return await base.DeleteAsync(rd => rd.DeptId == deptId);
        }
    }
}