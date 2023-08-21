//负责数据仓库的属性映射的具体逻辑实现
//使用字典类型来包含一系列属性映射的关系

using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;
using System.Diagnostics;
using System.Reflection;

namespace FakeXiecheng.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _touristRoutePropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
                       { "Id", new PropertyMappingValue(new List<string>(){ "Id" }) },
                       { "Title", new PropertyMappingValue(new List<string>(){ "Title" })},
                       { "Rating", new PropertyMappingValue(new List<string>(){ "Rating" })},
                       { "OriginalPrice", new PropertyMappingValue(new List<string>(){ "OriginalPrice" })},
           };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<TouristRouteDto, TouristRoute>(_touristRoutePropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            //获取匹配的映射对象
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }
            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)})");
        }

        public bool IsMappingExists<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            if (string.IsNullOrWhiteSpace(fields)) return true;//表示默认排序

            var fieldsAfterSplit = fields.Split(',');
            foreach ( var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(' ');
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);
                if(!propertyMapping.ContainsKey(propertyName)) return false;
            }
            return true;
        }

        public bool IsPropertiesExists<TSource>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields)) return true;
            var fieldsAfterSplit = fields.Split(',');
            foreach ( var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(TSource).GetProperty
                    (propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propertyInfo == null) return false;
            }
            return true;
        }
    }
}
