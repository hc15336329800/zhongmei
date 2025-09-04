namespace RuoYi.System.Repositories;
public class SysDeptRepository : BaseRepository<SysDept, SysDeptDto>
{
    public SysDeptRepository(ISqlSugarRepository<SysDept> sqlSugarRepository)
    {
        Repo = sqlSugarRepository;
    }

    public override ISugarQueryable<SysDept> Queryable(SysDeptDto dto)
    {
        return Repo.AsQueryable().Where((d) => d.DelFlag == DelFlag.No).WhereIF(dto.DeptId > 0, (d) => d.DeptId == dto.DeptId).WhereIF(dto.ParentId > 0, (d) => d.ParentId == dto.ParentId).WhereIF(!string.IsNullOrEmpty(dto.DelFlag), (d) => d.DelFlag == dto.DelFlag).WhereIF(!string.IsNullOrEmpty(dto.DeptName), (d) => d.DeptName!.Contains(dto.DeptName!)).WhereIF(!string.IsNullOrEmpty(dto.Status), (d) => d.Status == dto.Status);
    }

    public override ISugarQueryable<SysDeptDto> DtoQueryable(SysDeptDto dto)
    {
        return Repo.AsQueryable().LeftJoin<SysRoleDept>((d, rd) => d.DeptId == rd.DeptId).Where((d) => d.DelFlag == DelFlag.No).WhereIF(dto.DeptId > 0, (d) => d.DeptId == dto.DeptId).WhereIF(dto.ParentId > 0, (d) => d.ParentId == dto.ParentId).WhereIF(dto.ParentIds!.IsNotEmpty(), (d) => dto.ParentIds!.Contains(d.ParentId)).WhereIF(!string.IsNullOrEmpty(dto.DelFlag), (d) => d.DelFlag == dto.DelFlag).WhereIF(!string.IsNullOrEmpty(dto.Status), (d) => d.Status == dto.Status).WhereIF(!string.IsNullOrEmpty(dto.DeptName), (d) => d.DeptName!.Contains(dto.DeptName!)).WhereIF(dto.DeptCheckStrictly ?? false, (d) => d.DeptId != SqlFunc.Subqueryable<SysDept>().InnerJoin<SysRoleDept>((d1, rd1) => d1.DeptId == rd1.DeptId).Where((d1, rd1) => rd1.RoleId == dto.RoleId).GroupBy(d1 => d1.ParentId).Select(d1 => d1.ParentId)).Select((d) => new SysDeptDto { DeptId = d.DeptId, }, true);
    }

    protected override async Task FillRelatedDataAsync(IEnumerable<SysDeptDto> dtos)
    {
        if (dtos.IsEmpty())
            return;
        var parentIds = dtos.Where(d => d.ParentId.HasValue).Select(d => d.ParentId!.Value).Distinct().ToList();
        var parentDepts = await this.DtoQueryable(new SysDeptDto { ParentIds = parentIds }).ToListAsync();
        foreach (var dto in dtos)
        {
            dto.ParentName = parentDepts.FirstOrDefault(p => p.DeptId == dto.ParentId)?.DeptName;
        }
    }

    public async Task<List<SysDept>> GetDeptListAsync(SysDeptDto dto)
    {
        dto.DelFlag = DelFlag.No;
        return await base.GetListAsync(dto);
    }

    public async Task<List<long>> GetDeptListByRoleIdAsync(long roleId, bool isDeptCheckStrictly)
    {
        SysDeptDto query = new SysDeptDto
        {
            RoleId = roleId,
            DeptCheckStrictly = isDeptCheckStrictly
        };
        var list = await base.GetDtoListAsync(query);
        return list.Where(d => d.DeptId.HasValue).Select(d => d.DeptId!.Value).Distinct().ToList();
    }

    public async Task<int> CountNormalChildrenDeptByIdAsync(long deptId)
    {
        return await base.CountAsync(d => d.DelFlag == DelFlag.No && d.Status == "0" && SqlFunc.SplitIn(d.Ancestors, deptId.ToString()));
    }

    public async Task<List<SysDept>> GetChildrenDeptByIdAsync(long deptId)
    {
        var queryable = Repo.AsQueryable().Where(d => SqlFunc.SplitIn(d.Ancestors, deptId.ToString()));
        return await queryable.ToListAsync();
    }

    public async Task<int> UpdateDeptStatusNormalAsync(IEnumerable<long> deptIds)
    {
        return await base.Updateable().SetColumns(col => col.Status == Status.Enabled).Where(col => deptIds.Contains(col.DeptId)).ExecuteCommandAsync();
    }

    public async Task<bool> HasChildByDeptIdAsync(long parentDeptId)
    {
        var query = new SysDeptDto
        {
            DelFlag = DelFlag.No,
            ParentId = parentDeptId
        };
        return await base.AnyAsync(query);
    }

    public async Task<int> DeleteDeptByIdAsync(long deptId)
    {
        return await base.Updateable().SetColumns(col => col.DelFlag == DelFlag.Yes).Where(col => col.DeptId == deptId).ExecuteCommandAsync();
    }
}