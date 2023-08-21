using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Profiles
{
    public class TouristRouteProfile : Profile
    {
        //映射配置就是在这个类的构建函数中完成的
        //AutoMapper在完成依赖注入后会自动寻找Profiles文件夹，扫描其中的文件，在Profile的构造函数中完成对对象关系映射的配置
        //
        public TouristRouteProfile()
        {
            //创建TouristRoute对TouristRouteDto的映射，会自动映射两个对象中名称相同的字段
            CreateMap<TouristRoute, TouristRouteDto>()
                //数据投影：把资源对象中的数据经过一定的变化传递给目标对象
                .ForMember
                (
                dest => dest.Price,
                opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPresent ?? 1))
                )
                .ForMember
                (
                dest => dest.TravelDays,
                opt => opt.MapFrom(src => src.TravelDays.ToString())
                )
                .ForMember
                (
                dest => dest.TripType,
                opt => opt.MapFrom(src => src.TripType.ToString())
                )
                .ForMember
                (
                dest => dest.DepartureCity,
                opt => opt.MapFrom(src => src.DepartureCity.ToString())
                );

            CreateMap<TouristRoute, TouristRoutesListDto>().ForMember
                (
                dest => dest.Price,
                opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPresent ?? 1))
                );

            CreateMap<TouristRouteForCreationDto, TouristRoute>()
                .AddTransform<string>(s => s ?? string.Empty)
                .ForMember
                (
                dest => dest.Id,
                opt => opt.MapFrom(src => Guid.NewGuid())
                );

            CreateMap<TouristRouteForUpdateDto, TouristRoute>()
                .AddTransform<string>(s => s ?? string.Empty);

            CreateMap<TouristRoute, TouristRouteForUpdateDto>();

            CreateMap<TouristRoute, TouristRouteSimplifyDto>()
                .ForMember
                (
                dest => dest.Price,
                opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPresent ?? 1))
                );

        }
    }
}
