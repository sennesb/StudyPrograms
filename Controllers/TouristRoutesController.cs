using AutoMapper;
using Azure;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Helper;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using System.Dynamic;
using System.Net.Security;
using System.Reflection.Metadata;
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
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;

        //ITouristRouteRepository和IMapper这两个依赖项是在Startup中注入的，当接受一个匹配了TouristRoutesController路由的URL的HTTP请求时，
        //ASP.NET Core框架会自动解析ITouristRouteRepository和IMapper这两个依赖项，并将它们传递给构造函数，创建一个新的TouristRoutesController实例来处理该请求
        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper, IUrlHelperFactory urlHelperFactory, 
            IActionContextAccessor actionContextAccessor,
            IPropertyMappingService propertyMappingService)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext) ;//注入urlHelper服务
            _propertyMappingService = propertyMappingService;
        }


        //生成旅游路线资源url
        private string GenerateTouristRouteResourceURL(
            TouristRouteResourceParamaters paramaters, 
            PaginationResourceParamaters paramaters2,
            ResourceUrlType type)
        {
            return type switch
            {
                ResourceUrlType.PreviousPage => _urlHelper.Link("GetTouristRoutes",
                new
                {
                    fields = paramaters.Fields,
                    orderBy = paramaters.OrderBy,
                    keyword = paramaters.Keyword,
                    rating = paramaters.Rating,
                    pageNumber = paramaters2.PageNumber - 1,
                    pageSize = paramaters2.PageSize
                }),
                ResourceUrlType.NextPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = paramaters.Fields,
                        orderBy = paramaters.OrderBy,
                        keyword = paramaters.Keyword,
                        rating = paramaters.Rating,
                        pageNumber = paramaters2.PageNumber + 1,
                        pageSize = paramaters2.PageSize
                    }),
                _ => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = paramaters.Fields,
                        orderBy = paramaters.OrderBy,
                        keyword = paramaters.Keyword,
                        rating = paramaters.Rating,
                        pageNumber = paramaters2.PageNumber,
                        pageSize = paramaters2.PageSize
                    })
            };
        }

        //给单独资源加入hatoeas支持
        private IEnumerable<LinkDto> CreateLinkForTouristRoute(Guid touristRouteId, string fields)
        {
            var links = new List<LinkDto>();
            links.Add(
                new LinkDto(
                    Url.Link("GetTouristRouteById", new { touristRouteId, fields }),
                    "self", "GET"
                    )
                );
            links.Add(
                new LinkDto(
                    Url.Link("UpdateTouristRoute", new { touristRouteId }),
                    "update", "PUT"
                    )
                );
            links.Add(
                new LinkDto(
                    Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }),
                    "partially_update", "PATCh"
                    )
                );
            links.Add(
                new LinkDto(
                    Url.Link("DeleteTouristRoute", new { touristRouteId }),
                    "delete", "DELETE"
                    )
                );
            links.Add(
                new LinkDto(
                    Url.Link("GetPictureListForTouristRoute", new { touristRouteId }),
                    "get_picture", "GET"
                    )
                );
            links.Add(
                new LinkDto(
                    Url.Link("CreateTouristRoutePicture", new { touristRouteId }),
                    "create_picture", "POST"
                    )
                );

            return links;
        }

        //给列表资源加入hatoeas支持
        private IEnumerable<LinkDto> CreateLinksForTouristRouteList(
           TouristRouteResourceParamaters paramaters,PaginationResourceParamaters paramaters2)
        {
            var links = new List<LinkDto>();
            //添加self自我连接
            links.Add(new LinkDto
                (
                GenerateTouristRouteResourceURL(paramaters, paramaters2, ResourceUrlType.CurrentPage),
                "self","GET"
                ));
            //
            links.Add(new LinkDto
                (
                Url.Link("CreateTouristRoute", null),
                "create_tourist_route", "POST"
                ));

            return links;
        }



        //针对特定api添加自定义媒体类型
        [Produces(
            "application/json",
            "application/vnd.api.hatoeas+json",
            "application/vnd.api.touristRoute.simplify+json",
            "application/vnd.api.touristRoute.simplify.hatoeas+json"
            )]
        [HttpGet(Name = "GetTouristRoutes")]//表示处理get请求
        [HttpHead]//同时也是head请求，但head请求不能提高api的性能
        //[FromQuery]可省略，如果方法参数的名称与查询参数的名称相匹配，asp会自动将查询参数的值绑定到方法参数上，如果不一致就需要用[FromQuery(Name="")]来匹配。
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParamaters paramaters,
            [FromQuery] PaginationResourceParamaters paramaters2,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediatype))
            {
                return BadRequest();
            }
            if(!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(paramaters.OrderBy))
            {
                return BadRequest("请输入正确排序参数");
            }
            if (!_propertyMappingService.IsPropertiesExists<TouristRouteDto>(paramaters.Fields))
            {
                return BadRequest("请输入正确塑形参数");
            } 

            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(
                paramaters.Keyword, paramaters.RatingOperator, paramaters.RatingValue, paramaters.OrderBy,
                paramaters2.PageSize, paramaters2.PageNumber);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0){return NotFound("没有旅游路线");}
            
            //如果存在上一页或下一页就用url生成器生成相应url字符串，否则设为null
            var previousPageLink = touristRoutesFromRepo.HasPrevious ? GenerateTouristRouteResourceURL
                (paramaters, paramaters2, ResourceUrlType.PreviousPage) : null;
            var nextPageLink = touristRoutesFromRepo.HasNext ? GenerateTouristRouteResourceURL
                (paramaters,paramaters2,ResourceUrlType.NextPage) : null;
            //给响应头部加上自定义响应信息x-pagintion
            var pagintionMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPages= touristRoutesFromRepo.TotalPages
            };
            Response.Headers.Add("x-pagintion",
                // 第二个参数要先序列化为Json数据
                Newtonsoft.Json.JsonConvert.SerializeObject(pagintionMetadata) );

            bool isHatoeas = parsedMediatype.SubTypeWithoutSuffix.EndsWith("hatoeas", StringComparison.InvariantCultureIgnoreCase);
            var primarvMediaType = isHatoeas ? 
                parsedMediatype.SubTypeWithoutSuffix.Substring
                (0, parsedMediatype.SubTypeWithoutSuffix.Length - 8) : 
                parsedMediatype.SubTypeWithoutSuffix;
            //var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            //var shapedDtoList = touristRoutesDto.ShapDatas(paramaters.Fields);
            IEnumerable<object> touristRoutesDto;
            IEnumerable<ExpandoObject> shapedDtoList;
            if (primarvMediaType == "vnd.api.touristRoute.simplify")
            {
                touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteSimplifyDto>>(touristRoutesFromRepo);
                shapedDtoList = ((IEnumerable<TouristRouteSimplifyDto>)touristRoutesDto).ShapDatas(paramaters.Fields);
            }
            else
            {
                touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
                shapedDtoList = ((IEnumerable<TouristRouteDto>)touristRoutesDto).ShapDatas(paramaters.Fields);
            }

            if (isHatoeas)
            {
                var linkDto = CreateLinksForTouristRouteList(paramaters, paramaters2);
                var shapedDtoWithLinklist = shapedDtoList.Select(t =>
                {
                    var touristRouteDictionary = t as IDictionary<string, object>;
                    var links = CreateLinkForTouristRoute((Guid)touristRouteDictionary["Id"], null);
                    touristRouteDictionary.Add("links", links);
                    return touristRouteDictionary;
                });
                var result = new {value = shapedDtoWithLinklist, links = linkDto};
                return Ok(result);
            }
            return Ok(shapedDtoList);

        }
        /*[HttpHead]
        public IActionResult GetTouristRoutes()
        {
            ...
            return Ok();//和get代码一样，只是没有响应主体，asp.net mvc框架提供了简单的方法，在get请求加上[HttpHead]标记就行了
        }*/

        [HttpGet("touristrouteslist")]
        public async Task<IActionResult> GetTouristRoutesList(
            [FromQuery] PaginationResourceParamaters paramater2)
        {
            var touristRoutesListFromRepo = await _touristRouteRepository.GetTouristRoutesListAsync(paramater2.PageSize, paramater2.PageNumber, paramater2.All);
            if (touristRoutesListFromRepo == null || touristRoutesListFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRoutesListDto = _mapper.Map<IEnumerable<TouristRoutesListDto>>(touristRoutesListFromRepo);
            return Ok(touristRoutesListDto);
        }



        //api/touristroutes/{touristRouteId} 设计一个单一资源最佳实现方式是在url中使用目标资源的复数加/加资源Id
        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]//控制器路由会映射到api/touristroutes这部分，action函数只用负责/{touristRouteId}这部分的映射
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId, string? fields)
        {
            if(!_propertyMappingService.IsPropertiesExists<TouristRouteDto>(fields))
            {
                return BadRequest("请输入正确塑形参数");
            }
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"旅游路线{touristRouteId}找不到");
            }
            //使用automapper自动映射数据
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            //return Ok(touristRouteDto.ShapeData(fields));
            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fields);
            var result = touristRouteDto.ShapeData(fields) as IDictionary<string, object>;
            result.Add("links", linkDtos);
            return Ok(result);
        }


        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]//指定多角色验证时使用jwt Bearer的方式
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);//把新建的数据mapper成TouristRouteDto后传回给api作为数据输出

            var links = CreateLinkForTouristRoute(touristRouteModel.Id, null);
            var result = touristRouteToReturn.ShapeData(null) as IDictionary<string, object>;
            result.Add("links", links);

            //对于成功的Post应返回201，可以用CreatedAtRoute（路径名称，api路径参数，post成功后输出的数据）完成，
            return CreatedAtRoute("GetTouristRouteById", new { touristRouteId = result["Id"] }, result);
        }

        [HttpPost("CreateTouristRoutes/{num}")]
        public async Task<IActionResult> CreateTouristRoutesToTest([FromRoute]int num)
        {

            ICollection<TouristRoute> testRoutes = new List<TouristRoute> ();
            for (int i = 1; i <= num; i++)
            {
                var touristRouteDto = new TouristRouteForCreationDto
                {
                    Title = i.ToString(),
                    Description = i.ToString() + "hello",
                    Price = i
                };
                var touristRoute = _mapper.Map<TouristRoute>(touristRouteDto);
                testRoutes.Add(touristRoute);
            }
            _touristRouteRepository.AddTouristRoutesToTest(testRoutes);
            await _touristRouteRepository.SaveAsync();
            return Ok("成功添加");
        }


        [HttpPut("{touristRouteId}", Name = "UpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTouristRoute([FromRoute] Guid touristRouteId, [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线找不到");
            }
            var touristRouteFromrepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            //更新数据步骤：1.提取touristRouteFromrepo数据映射为Dto   2.更新Dto数据   3.映射回model
            //在数据仓库中调用的是底层框架EF，在EF中数据模型是根据上下文关系_context来追踪的，
            //执行_mapper.Map时，数据模型的数据其实已经被修改了，数据模型的追踪状态发生了变化，模型的追踪状态是_context自我管理的，
            //当执行数据库保存操作时，模型的追踪状态随着_context的保存，把更新后的状态写进数据库
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromrepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();//返回422
        }



        [HttpPatch("{touristRouteId}", Name = "PartiallyUpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        //使用JsonPatchDocument特殊类型前要进行依赖注入NewtonsoftJson
        public async Task<IActionResult> PartiallyUpdateTouristRoute([FromRoute] Guid touristRouteId, [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线找不到");
            }
            var touristRouteForRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteForRepo);
            patchDocument.ApplyTo(touristRouteToPatch, ModelState);
            //数据验证
            if (!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(touristRouteToPatch, touristRouteForRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }


        [HttpDelete("{touristRouteId}", Name = "DeleteTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute]Guid touristRouteId)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线找不到");
            }
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }


        [HttpDelete("({touristIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteByIDs(
            [ModelBinder( BinderType = typeof(ArrayModelBinder))]//应用自定义模型绑定器ArrayModelBinder,处理字符串类型的路由参数，并将其转换为 IEnumerable<Guid> 类型的参数。
            [FromRoute]IEnumerable<Guid> touristIDs)
        {
            if(touristIDs == null)return BadRequest();
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIDListAsync(touristIDs);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

    }
        
}
