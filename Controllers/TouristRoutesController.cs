using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

//ASP.NET api接口函数都叫action函数
//head请求没有返回值没有响应主体，只返回状态码
//head请求可以获取资源的头部信息，在支持缓存的系统中，这些信息可以用于检测获得的信息是否有效；还可以探测资源是否存在
//FromQuery负责接收url的参数，FromBody负责请求主体
namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private IMapper _mapper;

        //ITouristRouteRepository和IMapper这两个依赖项是在Startup中注入的，当接受一个匹配了TouristRoutesController路由的URL的HTTP请求时，
        //ASP.NET Core框架会自动解析ITouristRouteRepository和IMapper这两个依赖项，并将它们传递给构造函数，创建一个新的TouristRoutesController实例来处理该请求
        public TouristRoutesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }




        //给get请求添加过滤器用评分进行数据过滤
        [HttpGet]//表示处理get请求
        [HttpHead]//同时也是head请求，但head请求不能提高api的性能
        //[FromQuery]可省略，如果方法参数的名称与查询参数的名称相匹配，asp会自动将查询参数的值绑定到方法参数上，如果不一致就需要用[FromQuery(Name="")]来匹配。
        public IActionResult GetTouristRoutes([FromQuery] string keyword,
            string rating //评分筛选条件：小于，大于，等于
            )
        {
            Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");//正则表达式
            string operatorType;
            int raringVlaue;
            Match match = regex.Match(rating);
            if (match.Success)
            {
                operatorType = match.Groups[1].Value;
                raringVlaue = Int32.Parse(match.Groups[2].Value);
            }
            var touristRoutesFromRepo =_touristRouteRepository.GetTouristRoutes(keyword);
            if(touristRoutesFromRepo == null || touristRoutesFromRepo.Count()<=0)
            {
                return NotFound("没有旅游路线");
            }
            //
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            return Ok(touristRoutesFromRepo);
        }
        /*[HttpHead]
        public IActionResult GetTouristRoutes()
        {
            ...
            return Ok();//和get代码一样，只是没有响应主体，asp.net mvc框架提供了简单的方法，在get请求加上[HttpHead]标记就行了
        }*/




        //api/touristroutes/{touristRouteId} 设计一个单一资源最佳实现方式是在url中使用目标资源的复数加/加资源Id
        [HttpGet("{touristRouteId}")]//控制器路由会映射到api/touristroutes这部分，action函数只用负责/{touristRouteId}这部分的映射
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            if(touristRouteFromRepo == null)
            {
                return NotFound($"旅游路线{touristRouteId}找不到");
            }
            //使用automapper自动映射数据
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            return Ok(touristRouteDto);
        }
    }
}
