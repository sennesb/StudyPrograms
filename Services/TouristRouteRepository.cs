using FakeXiecheng.API.Database;
using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore;

//数据仓库，包含所有从数据库获取数据的操作
//IQueryable可以叠加处理linq语句，最后统一访问数据库，叫做延迟执行


namespace FakeXiecheng.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;
        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }

        public TouristRoute GetTouristRoute(Guid touristRouteId)
        {
            return _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefault(n => n.Id == touristRouteId);
        }

        public IEnumerable<TouristRoute> GetTouristRoutes(string keyword )
        {
            //IQueryable这一步只是生成了sql语句，并没有执行数据库查询操作
            IQueryable<TouristRoute> result = _context
                .TouristRoutes
                .Include(t => t.TouristRoutePictures);//在实际中可能不可避免要在获取父资源的同时获取子资源,用Include通过外键连接两张表，实现数据的立即加载  
            if (!string.IsNullOrWhiteSpace(keyword))//如果keyword不等于null或空字符串时
            {
                keyword = keyword.Trim();
                result = result.Where(t => t.Title.Contains(keyword));//Title中包含keyword的
            }
            //ToList是IQueryable的内嵌函数，通过调用它IQueryable就会马上执行数据库的访问操作，
            //和FirstOrDefault类似，但FirstOrDefault处理的是单独的数据，ToList处理的是列表类型的数据
            return result.ToList();
        }

        public bool TouristRouteExists(Guid touristRouteId)
        {
            return _context.TouristRoutes.Any(t => t.Id == touristRouteId);//使用Any函数判断是否存在
        }

        public IEnumerable<TouristRoutePicture> GetPicturesByTouristRouteId(Guid touristRouteId)
        {
            return _context.TouristRoutesPictures.Where(p => p.TouristRouteId == touristRouteId).ToList();
        }

        public TouristRoutePicture GetPicture(int  pictureId)
        {
            return _context.TouristRoutesPictures.Where(p => p.Id == pictureId).FirstOrDefault();
        }
    }
}
