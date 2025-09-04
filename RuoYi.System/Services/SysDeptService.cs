using RuoYi.Common.Constants;
using RuoYi.Common.Interceptors;
using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.Framework.Exceptions;
using RuoYi.System.Repositories;

namespace RuoYi.System.Services;
public class SysDeptService : BaseService<SysDept, SysDeptDto>, ITransient
{
    private readonly ILogger<SysDeptService> _logger;
    private readonly SysDeptRepository _sysDeptRepository;
    private readonly SysUserRepository _sysUserRepository;
    private readonly SysRoleRepository _sysRoleRepository;
    public SysDeptService(ILogger<SysDeptService> logger, SysDeptRepository sysDeptRepository, SysUserRepository sysUserRepository, SysRoleRepository sysRoleRepository)
    {
        BaseRepo = sysDeptRepository;
        _logger = logger;
        _sysDeptRepository = sysDeptRepository;
        _sysUserRepository = sysUserRepository;
        _sysRoleRepository = sysRoleRepository;
    }

    public async Task<SysDept> GetAsync(long id)
    {
        return await base.FirstOrDefaultAsync(e => e.DeptId == id);
    }

    public async Task<SysDeptDto> GetDtoAsync(long id)
    {
        var entity = await GetAsync(id);
        return entity.Adapt<SysDeptDto>();
    }

    [DataScope(DeptAlias = "d")]
    public override async Task<List<SysDeptDto>> GetDtoListAsync(SysDeptDto dto)
    {
        return await _sysDeptRepository.GetDtoListAsync(dto);
    }

    public async Task<List<long>> GetDeptListByRoleIdAsync(long roleId)
    {
        SysRole role = _sysRoleRepository.GetRoleById(roleId);
        return await _sysDeptRepository.GetDeptListByRoleIdAsync(roleId, role.DeptCheckStrictly);
    }

    public async Task<int> CountNormalChildrenDeptByIdAsync(long deptId)
    {
        return await _sysDeptRepository.CountNormalChildrenDeptByIdAsync(deptId);
    }

#region TreeSelect
    public async Task<List<TreeSelect>> GetDeptTreeListAsync(SysDeptDto dto)
    {
        List<SysDeptDto> depts = await this.GetDtoListAsync(dto);
        return BuildDeptTreeSelect(depts);
    }

    private List<TreeSelect> BuildDeptTreeSelect(List<SysDeptDto> depts)
    {
        List<SysDeptDto> deptTrees = BuildDeptTree(depts);
        return deptTrees.Select(dept => new TreeSelect(dept)).ToList();
    }

    private List<SysDeptDto> BuildDeptTree(List<SysDeptDto> depts)
    {
        List<SysDeptDto> returnList = new List<SysDeptDto>();
        List<long> tempList = depts.Where(d => d.DeptId.HasValue).Select(d => d.DeptId!.Value).ToList();
        foreach (SysDeptDto dept in depts)
        {
            if (dept.ParentId.HasValue && !tempList.Contains(dept.ParentId.Value))
            {
                RecursionFn(depts, dept);
                returnList.Add(dept);
            }
        }

        if (returnList.IsEmpty())
        {
            returnList = depts;
        }

        return returnList;
    }

    private void RecursionFn(List<SysDeptDto> list, SysDeptDto t)
    {
        List<SysDeptDto> childList = GetChildList(list, t);
        t.Children = childList;
        foreach (SysDeptDto tChild in childList)
        {
            if (HasChild(list, tChild))
            {
                RecursionFn(list, tChild);
            }
        }
    }

    private List<SysDeptDto> GetChildList(List<SysDeptDto> list, SysDeptDto t)
    {
        List<SysDeptDto> tList = new List<SysDeptDto>();
        foreach (SysDeptDto n in list)
        {
            if (n.ParentId > 0 && n.ParentId == t.DeptId)
            {
                tList.Add(n);
            }
        }

        return tList;
    }

    public async Task<bool> HasChildByDeptIdAsync(long deptId)
    {
        return await _sysDeptRepository.HasChildByDeptIdAsync(deptId);
    }

    public async Task<bool> CheckDeptExistUserAsync(long deptId)
    {
        return await _sysUserRepository.CheckDeptExistUserAsync(deptId);
    }

    private bool HasChild(List<SysDeptDto> list, SysDeptDto t)
    {
        return GetChildList(list, t).Count > 0;
    }

#endregion
    public async Task<bool> CheckDeptNameUniqueAsync(SysDeptDto dept)
    {
        SysDept info = await _sysDeptRepository.GetFirstAsync(new SysDeptDto { DeptName = dept.DeptName, ParentId = dept.ParentId });
        if (info != null && info.DeptId != dept.DeptId)
        {
            return UserConstants.NOT_UNIQUE;
        }

        return UserConstants.UNIQUE;
    }

    public async Task CheckDeptDataScopeAsync(long deptId)
    {
        if (!SecurityUtils.IsAdmin())
        {
            SysDeptDto dto = new SysDeptDto
            {
                DeptId = deptId
            };
            List<SysDept> depts = await _sysDeptRepository.GetDeptListAsync(dto);
            if (depts.IsEmpty())
            {
                throw new ServiceException("没有权限访问部门数据！");
            }
        }
    }

    public async Task<bool> InsertDeptAsync(SysDeptDto dept)
    {
        SysDept info = await _sysDeptRepository.FirstOrDefaultAsync(d => d.DeptId == dept.ParentId);
        if (!UserConstants.DEPT_NORMAL.Equals(info.Status))
        {
            throw new ServiceException("部门停用，不允许新增");
        }

        dept.Ancestors = info.Ancestors + "," + dept.ParentId;
        dept.DelFlag = DelFlag.No;
        return await _sysDeptRepository.InsertAsync(dept);
    }

    public async Task<int> UpdateDeptAsync(SysDeptDto dept)
    {
        SysDept newParentDept = await this.GetAsync(dept.ParentId.Value);
        SysDept oldDept = await this.GetAsync(dept.DeptId.Value);
        if (newParentDept != null && oldDept != null)
        {
            string newAncestors = newParentDept.Ancestors + "," + newParentDept.DeptId;
            string oldAncestors = oldDept.Ancestors!;
            dept.Ancestors = newAncestors;
            await UpdateDeptChildrenAsync(dept.DeptId.Value, newAncestors, oldAncestors);
        }

        int result = await _sysDeptRepository.UpdateAsync(dept, true);
        if (UserConstants.DEPT_NORMAL.Equals(dept.Status) && StringUtils.IsNotEmpty(dept.Ancestors) && !StringUtils.Equals("0", dept.Ancestors))
        {
            await UpdateParentDeptStatusNormalAsync(dept);
        }

        return result;
    }

    public async Task UpdateDeptChildrenAsync(long deptId, string newAncestors, string oldAncestors)
    {
        List<SysDept> children = await _sysDeptRepository.GetChildrenDeptByIdAsync(deptId);
        foreach (SysDept child in children)
        {
            child.Ancestors = child.Ancestors!.ReplaceFirst(oldAncestors, newAncestors);
        }

        if (children.Count > 0)
        {
            await _sysDeptRepository.UpdateAsync(children);
        }
    }

    private async Task UpdateParentDeptStatusNormalAsync(SysDeptDto dept)
    {
        string ancestors = dept.Ancestors!;
        long[] deptIds = ConvertUtils.ToLongArray(ancestors);
        await _sysDeptRepository.UpdateDeptStatusNormalAsync(deptIds);
    }

    public async Task<int> DeleteDeptByIdAsync(long deptId)
    {
        return await _sysDeptRepository.DeleteDeptByIdAsync(deptId);
    }
}