

//IQueryable拓展类

using FakeXiecheng.API.Services;
using System.Linq.Dynamic.Core;

namespace FakeXiecheng.API.Helper
{
    public static class IQueryableExtensions
    {
        //this IQueryable<T> source是一个扩展方法的参数声明,指定了该扩展方法可以应用于类型为IQueryable<T>的对象，并将该对象作为方法的第一个参数。
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (mappingDictionary == null) throw new ArgumentNullException("mappingDictionary");
            //if (string.IsNullOrWhiteSpace(orderBy)) return source;

            var orderByString = string.Empty;
            var orderByAfterSplit = orderBy.Split(',');
            foreach(var order in orderByAfterSplit)
            {
                var trimmedOrder = order.Trim();
                //通过“desc”判断升序还是降序
                var orderDescending = trimmedOrder.EndsWith(" desc");
                //删除升序降序字符串“asc”、“desc”来获取属性名称
                var indexOfFirstSpace = trimmedOrder.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1
                    ? trimmedOrder : trimmedOrder.Remove(indexOfFirstSpace);

                if (!(mappingDictionary.ContainsKey(propertyName)))
                {
                    throw new ArgumentException($"key mapping for {propertyName} is missing");
                }

                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }
                foreach(var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                {
                    //给IQueryable添加排序字符串
                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                        + destinationProperty
                        + (orderDescending ? " descending" : " ascending");
                }
            }
            //这步用到了System.Linq.Dynamic.Core,可以在LINQ查询中使用字符串来表示排序规则
            return source.OrderBy(orderByString);
        }
    }
}
