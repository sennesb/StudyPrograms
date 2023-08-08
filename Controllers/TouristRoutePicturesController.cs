using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private IMapper _mapper;

        public TouristRoutePicturesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper
        )
        {
            _touristRouteRepository = touristRouteRepository ??
                throw new ArgumentNullException(nameof(touristRouteRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetPictureListForTouristRoute(Guid touristRouteId)//touristRouteId来自Http请求url里的{touristRouteId}
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("旅游路线不存在");
            }

            var picturesFromRepo = _touristRouteRepository.GetPicturesByTouristRouteId(touristRouteId);
            if (picturesFromRepo == null || picturesFromRepo.Count() <= 0)
            {
                return NotFound("图片不存在");
            }
            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFromRepo));
        }

        //获取单独图片
        //在设计Ruslful api时，处理有父子或嵌套关系的资源的时候，要先获取父资源再在其基础上获取子资源，如果父资源都不存在最好不要暴露子资源
        [HttpGet("{pictureId}")]
        public IActionResult GetPicture(Guid touristRouteId,int pictureId)
        {
            //判断父资源
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("旅游路线不存在");
            }

            var pictureFormRepo = _touristRouteRepository.GetPicture(pictureId);
            if(pictureFormRepo == null)
            {
                return NotFound("这张图片不存在");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFormRepo));
        }


        }
}
