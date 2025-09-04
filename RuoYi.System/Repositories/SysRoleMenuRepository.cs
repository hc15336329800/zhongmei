using RuoYi.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;

namespace RuoYi.System.Repositories
{
    public class SysRoleMenuRepository : BaseRepository<SysRoleMenu, SysRoleMenuDto>
    {
        public SysRoleMenuRepository(ISqlSugarRepository<SysRoleMenu> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<SysRoleMenu> Queryable(SysRoleMenuDto dto)
        {
            return Repo.AsQueryable().WhereIF(dto.MenuId > 0, (t) => t.MenuId == dto.MenuId).WhereIF(dto.RoleId > 0, (t) => t.RoleId == dto.RoleId);
        }

        public override ISugarQueryable<SysRoleMenuDto> DtoQueryable(SysRoleMenuDto dto)
        {
            return Repo.AsQueryable().WhereIF(dto.MenuId > 0, (t) => t.MenuId == dto.MenuId).WhereIF(dto.RoleId > 0, (t) => t.RoleId == dto.RoleId).Select((t) => new SysRoleMenuDto { MenuId = t.MenuId, RoleId = t.RoleId });
        }

        public bool CheckMenuExistRole(long menuId)
        {
            return base.Count(r => r.MenuId == menuId) > 0;
        }

        public async Task<int> DeleteByRoleIdAsync(long roleId)
        {
            return await base.DeleteAsync(m => m.RoleId == roleId);
        }

        public async Task<int> DeleteByRoleIdsAsync(List<long> roleIds)
        {
            return await base.DeleteAsync(m => roleIds.Contains(m.RoleId));
        }
    }
}