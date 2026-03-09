using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiCriteriaSort.Helpers
{
    public static class SortingExtensions
    {
        public static IEnumerable<T> MultipleSort<T>(this IEnumerable<T> source, List<SortCriteria> criteriaList)
        {
            if (source == null || criteriaList == null || !criteriaList.Any())
            {
                return source;
            }

            IOrderedEnumerable<T> orderedResult = null;

            for (int i = 0; i < criteriaList.Count; i++)
            {
                var criterion = criteriaList[i];
                PropertyInfo propertyInfo = typeof(T).GetProperty(criterion.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    continue;
                }

                if (i == 0)
                {
                    if (criterion.Direction == SortDirection.Ascending)
                    {
                        orderedResult = source.OrderBy(x => propertyInfo.GetValue(x, null));
                    }
                    else
                    {
                        orderedResult = source.OrderByDescending(x => propertyInfo.GetValue(x, null));
                    }
                }
                else
                {
                    if (criterion.Direction == SortDirection.Ascending)
                    {
                        orderedResult = orderedResult.ThenBy(x => propertyInfo.GetValue(x, null));
                    }
                    else
                    {
                        orderedResult = orderedResult.ThenByDescending(x => propertyInfo.GetValue(x, null));
                    }
                }
            }

            return orderedResult ?? source;
        }
    }
}
