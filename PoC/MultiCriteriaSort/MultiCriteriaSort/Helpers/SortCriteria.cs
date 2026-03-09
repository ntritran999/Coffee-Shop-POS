using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCriteriaSort.Helpers
{
    public class SortCriteria
    {
        public string PropertyName { get; set; }
        public SortDirection Direction { get; set; }

        public SortCriteria(string propertyName, SortDirection direction = SortDirection.Ascending)
        {
            PropertyName = propertyName;
            Direction = direction;
        }
    }
}
