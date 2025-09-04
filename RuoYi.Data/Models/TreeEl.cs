using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuoYi.Data.Models
{
    public class TreeEl<T>
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public List<TreeEl<T>>? Children { get; set; }

        public TreeEl(T entity, Func<T, long> idSelector, Func<T, string> labelSelector, Func<T, List<T>> childrenSelector)
        {
            Id = idSelector(entity);
            Label = labelSelector(entity);
            var childs = childrenSelector(entity);
            if (childs != null && childs.Any())
            {
                Children = childs.Select(c => new TreeEl<T>(c, idSelector, labelSelector, childrenSelector)).ToList();
            }
        }

        public bool ShouldSerializeChildren() => Children != null && Children.Any();
    }
}