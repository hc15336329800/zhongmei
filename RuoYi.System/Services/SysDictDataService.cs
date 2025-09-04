using RuoYi.Common.Utils;
using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysDictDataService : BaseService<SysDictData, SysDictDataDto>, ITransient
{
    private readonly ILogger<SysDictDataService> _logger;
    private readonly SysDictDataRepository _sysDictDataRepository;
    public SysDictDataService(ILogger<SysDictDataService> logger, SysDictDataRepository sysDictDataRepository)
    {
        BaseRepo = sysDictDataRepository;
        _logger = logger;
        _sysDictDataRepository = sysDictDataRepository;
    }

    public async Task<SysDictData> GetAsync(long dictCode)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.DictCode == dictCode);
        return entity;
    }

    public async Task<bool> InsertDictDataAsync(SysDictDataDto data)
    {
        bool success = await _sysDictDataRepository.InsertAsync(data);
        if (success)
        {
            List<SysDictData> dictDatas = await _sysDictDataRepository.SelectDictDataByTypeAsync(data.DictType!);
            DictUtils.SetDictCache(data.DictType!, dictDatas);
        }

        return success;
    }

    public async Task<int> UpdateDictDataAsync(SysDictDataDto data)
    {
        int row = await _sysDictDataRepository.UpdateAsync(data);
        if (row > 0)
        {
            List<SysDictData> dictDatas = await _sysDictDataRepository.SelectDictDataByTypeAsync(data.DictType!);
            DictUtils.SetDictCache(data.DictType!, dictDatas);
        }

        return row;
    }

    public async Task DeleteDictDataByIdsAsync(long[] dictCodes)
    {
        foreach (long dictCode in dictCodes)
        {
            SysDictData data = await GetAsync(dictCode);
            await _sysDictDataRepository.DeleteAsync(dictCode);
            List<SysDictData> dictDatas = await _sysDictDataRepository.SelectDictDataByTypeAsync(data.DictType!);
            DictUtils.SetDictCache(data.DictType!, dictDatas);
        }
    }
}