
//PropertyMappingService字典中对应的的“值”

namespace FakeXiecheng.API.Services
{
    public class PropertyMappingValue
    {
        //将要被映射的目标属性
        public IEnumerable<string> DestinationProperties { get;private set; }

        public PropertyMappingValue(IEnumerable<string> destinationProperties)
        {
            DestinationProperties = destinationProperties;
        }
    }
}
