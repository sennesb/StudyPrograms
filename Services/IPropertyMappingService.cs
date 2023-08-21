//负责数据仓库的属性映射的具体逻辑实现，还有参数的检查
//使用字典类型来包含一系列属性映射的关系

namespace FakeXiecheng.API.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool IsMappingExists<TSource, TDestination>(string fields);
        bool IsPropertiesExists<T>(string fields);
    }
}