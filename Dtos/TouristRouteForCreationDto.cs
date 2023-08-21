
//数据输入Dto，和数据输出Dto尽量保持独立，以便model变化时减少对UI层的影响
//Body中的数据时json格式的，所以使用Dto来反序列化输入数据

using FakeXiecheng.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FakeXiecheng.API.Dtos
{
    
    public class TouristRouteForCreationDto : TouristRouteForManipulationDto
    {

    }
}
