using Mapster;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Constants;
using RuoYi.Common.Data;
using RuoYi.Data;
using RuoYi.Data.Entities;
using RuoYi.Data.Models;
using RuoYi.Device.Entities;
using RuoYi.Framework.DependencyInjection;
using RuoYi.Framework.Exceptions;
using RuoYi.Framework.Extensions;
using ZM.Device.Repositories;

namespace ZM.Device.Services
{
    public class DeviceTypeService : BaseService<DeviceType, DeviceTypeDto>, ITransient
    {
        private readonly ILogger<DeviceTypeService> _logger;
        private readonly DeviceTypeRepository _DeviceTypeRepository;
        public DeviceTypeService(ILogger<DeviceTypeService> logger, DeviceTypeRepository deviceTypeRepository)
        {
            _logger = logger;
            _DeviceTypeRepository = deviceTypeRepository;
            BaseRepo = deviceTypeRepository;
        }

        public async Task<bool> InsertDeptAsync(DeviceTypeDto dept)
        {
            DeviceType info = await _DeviceTypeRepository.FirstOrDefaultAsync(d => d.Id == dept.ParentId);
            if (!UserConstants.DEPT_NORMAL.Equals(info.Status))
            {
                throw new ServiceException("部门停用，不允许新增");
            }

            dept.Ancestors = info.Ancestors + "," + dept.ParentId;
            dept.DelFlag = DelFlag.No;
            return await _DeviceTypeRepository.InsertAsync(dept);
        }

        public async Task<int> DeleteDeptByIdAsync(long deptId)
        {
            return await _DeviceTypeRepository.DeleteDeptByIdAsync(deptId);
        }

        public async Task<int> UpdateDeptAsync(DeviceTypeDto dept)
        {
            DeviceType newParentDept = await this.GetAsync(dept.ParentId);
            DeviceType oldDept = await this.GetAsync(dept.Id);
            if (newParentDept != null && oldDept != null)
            {
                string newAncestors = newParentDept.Ancestors + "," + newParentDept.Id;
                string oldAncestors = oldDept.Ancestors!;
                dept.Ancestors = newAncestors;
                await UpdateDeptChildrenAsync(dept.Id, newAncestors, oldAncestors);
            }

            int result = await _DeviceTypeRepository.UpdateAsync(dept, true);
            return result;
        }

        public async Task<DeviceType> GetAsync(long id)
        {
            return await base.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<DeviceTypeDto> GetDtoAsync(long id)
        {
            var entity = await GetAsync(id);
            return entity.Adapt<DeviceTypeDto>();
        }

        public async Task UpdateDeptChildrenAsync(long deptId, string newAncestors, string oldAncestors)
        {
            List<DeviceType> children = await _DeviceTypeRepository.GetChildrenDeptByIdAsync(deptId);
            foreach (DeviceType child in children)
            {
                child.Ancestors = child.Ancestors!.ReplaceFirst(oldAncestors, newAncestors);
            }

            if (children.Count > 0)
            {
                await _DeviceTypeRepository.UpdateAsync(children);
            }
        }

        public async Task<bool> CheckDeptNameUniqueAsync(DeviceTypeDto dept)
        {
            DeviceType info = await _DeviceTypeRepository.GetFirstAsync(new DeviceTypeDto { DeptName = dept.DeptName, ParentId = dept.ParentId });
            if (info != null && info.Id != dept.Id)
            {
                return UserConstants.NOT_UNIQUE;
            }

            return UserConstants.UNIQUE;
        }

        public async Task<bool> HasChildByDeptIdAsync(long deptId)
        {
            return await _DeviceTypeRepository.HasChildByDeptIdAsync(deptId);
        }

        public async Task<bool> CheckDeptExistUserAsync(long deptId)
        {
            return await _DeviceTypeRepository.CheckDeviceTypeExistInManagementTypeAsync(deptId);
        }

#region EL树组件
        public async Task<List<TreeEl<DeviceTypeDto>>> GetTreeNodeAsync(DeviceTypeDto dto)
        {
            var depts = await _DeviceTypeRepository.GetDtoListAsync(dto);
            return BuildTreeNode(depts);
        }

        private List<TreeEl<DeviceTypeDto>> BuildTreeNode(List<DeviceTypeDto> depts)
        {
            var deptTrees = BuildTree(depts);
            return deptTrees.Select(dept => new TreeEl<DeviceTypeDto>(dept, d => d.Id, d => d.DeptName ?? string.Empty, d => d.Children ?? new List<DeviceTypeDto>())).ToList();
        }

        private List<DeviceTypeDto> BuildTree(List<DeviceTypeDto> depts)
        {
            var topNodes = depts.Where(d => d.ParentId == 0).ToList();
            foreach (var top in topNodes)
            {
                SetTreeChildrenNode(depts, top);
            }

            return topNodes.Any() ? topNodes : depts;
        }

        private void SetTreeChildrenNode(List<DeviceTypeDto> allNodes, DeviceTypeDto parent)
        {
            var children = allNodes.Where(n => n.ParentId == parent.Id).ToList();
            parent.Children = children;
            foreach (var child in children)
            {
                SetTreeChildrenNode(allNodes, child);
            }
        }
#endregion
    }
}