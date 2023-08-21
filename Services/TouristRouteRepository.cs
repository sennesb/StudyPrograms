using AutoMapper;
using FakeXiecheng.API.Database;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Helper;
using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore;

//数据仓库，包含所有从数据库获取数据的操作
//IQueryable可以叠加处理linq语句，最后统一访问数据库，叫做延迟执行


namespace FakeXiecheng.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRouteRepository(AppDbContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }



        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefaultAsync(n => n.Id == touristRouteId);
        }


        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesListAsync(int pageSize, int pageNumber, string all)
        {
            IQueryable<TouristRoute> result = _context.TouristRoutes.Include(t => t.TouristRoutePictures).OrderBy(r => r.Title);
            if(all == null)
            {
                var skip = (pageNumber - 1) * pageSize;
                result = result.Skip(skip);
                result = result.Take(pageSize);
            }
            return await result.ToListAsync();
        }
        

        public async Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(string keyword, string ratingOperator, int? raringVlaue, string orderBy, int pageSize, int pageNumber)
        {
            //IQueryable延迟执行，这一步只是生成了sql语句，并没有执行数据库查询操作
            IQueryable<TouristRoute> result = _context
                .TouristRoutes
                .Include(t => t.TouristRoutePictures);//在实际中可能不可避免要在获取父资源的同时获取子资源,用Include通过外键连接两张表，实现数据的立即加载  
            if (!string.IsNullOrWhiteSpace(keyword))//如果keyword不等于null或空字符串时
            {
                keyword = keyword.Trim();
                result = result.Where(t => t.Title.Contains(keyword));//Title中包含keyword的
            }
            if (raringVlaue >= 0)
            {
                switch (ratingOperator)
                {
                    case "largerThan":
                        result = result.Where(t => t.Rating >= raringVlaue);
                        break;
                    case "lessThan":
                        result = result.Where(t => t.Rating <= raringVlaue);
                        break;
                    case "equalTo":
                    default:
                        result = result.Where(t => t.Rating == raringVlaue);
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var touristRouteMappingDictionary = _propertyMappingService.GetPropertyMapping<TouristRouteDto, TouristRoute>();
                result = result.ApplySort(orderBy, touristRouteMappingDictionary);
            }



            //ToList是IQueryable的内嵌函数，通过调用它IQueryable就会马上执行数据库的访问操作，
            //和FirstOrDefault类似，但FirstOrDefault处理的是单独的数据，ToList处理的是列表类型的数据
            return await PaginationList<TouristRoute>.CreateAsync(pageNumber,pageSize,result);
        }


        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(t => ids.Contains(t.Id)).ToListAsync();
        }


        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(t => t.Id == touristRouteId);//使用Any函数判断是否存在
        }

        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutesPictures.Where(p => p.TouristRouteId == touristRouteId).ToListAsync();
        }

        public async Task<TouristRoutePicture> GetPictureAsync(int  pictureId)
        {
            return await _context.TouristRoutesPictures.Where(p => p.Id == pictureId).FirstOrDefaultAsync();
        }


        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if(touristRoute == null) throw new ArgumentNullException(nameof(touristRoute));

            _context.TouristRoutes.Add(touristRoute);
        }
        public void AddTouristRoutesToTest(ICollection<TouristRoute> testRoutes)
        {

            _context.TouristRoutes.AddRange(testRoutes);
        }

        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristRouteId == Guid.Empty)throw new ArgumentNullException(nameof(touristRouteId));
            if (touristRoutePicture == null)throw new ArgumentNullException(nameof(touristRoutePicture));
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutesPictures.Add(touristRoutePicture);
        }


        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }

        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }


        public void DeleteTouristRoutePicture(TouristRoutePicture picture)
        {
            _context.TouristRoutesPictures.Remove(picture);
        }


        public async Task<ShoppingCart> GetShoppingCartByUserIdAsync(string usreId)
        {
            return await _context.ShoppingCarts
                .Include(s => s.User)
                .Include(s => s.ShoppingCartItems)
                .ThenInclude(li => li.TouristRoute)
                .Where(s => s.UserId == usreId)
                .FirstOrDefaultAsync();
        }

        public async Task CreateShoppingCart(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        public async Task AddShoppingCartItemAsync(LineItem lineItem)
        {
            await _context.LineItems.AddAsync(lineItem);
        }


        public async Task<LineItem> GetShoppingCartItemByItemIdAsync(int lineItemId)
        {
            return await _context.LineItems.Where(li => li.Id == lineItemId).FirstOrDefaultAsync();
        }

        public void DeleteShoppingCartItem(LineItem lineItem)
        {
            _context.LineItems.Remove(lineItem);
        }


        public async Task<IEnumerable<LineItem>> GetShoppingCartItemsByIdListAsync(IEnumerable<int>ids)
        {
            return (await _context.LineItems.Where(li => ids.Contains(li.Id)).ToListAsync());
        }
        public void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems)
        {
            _context.LineItems.RemoveRange(lineItems);
        }


        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }


        public async Task<PaginationList<Order>> GetOrdersByUserIdAsync(string userId, int pageSize, int PageNumber)
        {
            IQueryable<Order> result = _context.Orders.Where(o => o.UserId == userId);
            return await PaginationList<Order>.CreateAsync(PageNumber,pageSize,result);
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.TouristRoute)
                .Where(o => o.Id == orderId).FirstOrDefaultAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }


    }
}
