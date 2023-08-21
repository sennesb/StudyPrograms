
//将会在数据仓库中使用的映射字典

namespace FakeXiecheng.API.Services
{
    //<TSource, TDestination>是用来确定具体属性映射的字典
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; set; }
        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary;
        }
    }
}
