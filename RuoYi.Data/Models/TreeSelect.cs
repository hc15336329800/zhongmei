using Newtonsoft.Json;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;

namespace RuoYi.Data.Models
{
    public class TreeSelect
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public List<TreeSelect>? Children { get; set; }

        public TreeSelect()
        {
        }

        public TreeSelect(SysDeptDto dept)
        {
            this.Id = dept.DeptId ?? 0;
            this.Label = dept.DeptName!;
            this.Children = dept.Children?.Select(c => new TreeSelect(c)).ToList();
        }

        public TreeSelect(SysMenu menu)
        {
            this.Id = menu.MenuId;
            this.Label = menu.MenuName!;
            this.Children = menu.Children?.Select(m => new TreeSelect(m)).ToList();
        }

        public bool ShouldSerializeChildren()
        {
            return Children != null && Children.Any();
        }
    }
}