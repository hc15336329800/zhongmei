namespace RuoYi.System.Repositories;
public class SysDictDataRepository : BaseRepository<SysDictData, SysDictDataDto>
{
    public SysDictDataRepository(ISqlSugarRepository<SysDictData> sqlSugarRepository)
    {
        Repo = sqlSugarRepository;
    }

    public override ISugarQueryable<SysDictData> Queryable(SysDictDataDto dto)
    {
        return Repo.AsQueryable().WhereIF(dto.DictCode > 0, (t) => t.DictCode == dto.DictCode).WhereIF(!string.IsNullOrEmpty(dto.DictType), (t) => t.DictType == dto.DictType).WhereIF(!string.IsNullOrEmpty(dto.DictLabel), (t) => t.DictLabel!.Contains(dto.DictLabel!)).WhereIF(!string.IsNullOrEmpty(dto.Status), (t) => t.Status == dto.Status);
    }

    public override ISugarQueryable<SysDictDataDto> DtoQueryable(SysDictDataDto dto)
    {
        return Repo.AsQueryable().WhereIF(dto.DictCode > 0, (t) => t.DictCode == dto.DictCode).WhereIF(!string.IsNullOrEmpty(dto.DictType), (t) => t.DictType == dto.DictType).WhereIF(!string.IsNullOrEmpty(dto.DictLabel), (t) => t.DictLabel!.Contains(dto.DictLabel!)).WhereIF(!string.IsNullOrEmpty(dto.Status), (t) => t.Status == dto.Status).Select((d) => new SysDictDataDto { CreateBy = d.CreateBy, CreateTime = d.CreateTime, UpdateBy = d.UpdateBy, UpdateTime = d.UpdateTime, DictCode = d.DictCode, DictSort = d.DictSort, DictLabel = d.DictLabel, DictValue = d.DictValue, DictType = d.DictType, CssClass = d.CssClass, ListClass = d.ListClass, IsDefault = d.IsDefault, Status = d.Status, Remark = d.Remark, });
    }

    protected override async Task FillRelatedDataAsync(IEnumerable<SysDictDataDto> dtos)
    {
        await base.FillRelatedDataAsync(dtos);
        foreach (var dto in dtos)
        {
            dto.StatusDesc = Status.ToDesc(dto.Status);
            dto.IsDefaultDesc = YesNo.ToDesc(dto.IsDefault);
        }
    }

    public async Task<List<SysDictData>> SelectDictDataByTypeAsync(string dictType)
    {
        if (string.IsNullOrEmpty(dictType))
            return null !;
        var query = new SysDictDataDto
        {
            DictType = dictType
        };
        return await base.GetListAsync(query);
    }

    public async Task<int> UpdateDictDataTypeAsync(string oldDictType, string newDictType)
    {
        return await base.Updateable().SetColumns(col => col.DictType == newDictType).Where(col => col.DictType == oldDictType).ExecuteCommandAsync();
    }

    public async Task<int> CountDictDataByTypeAsync(string dictType)
    {
        return await base.CountAsync(c => c.DictType == dictType);
    }
}