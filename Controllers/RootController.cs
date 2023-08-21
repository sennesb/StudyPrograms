using FakeXiecheng.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            //自我链接
            links.Add(new LinkDto(Url.Link("GetRoot",null),
                "self", "GET"));


            //一级链接

            //旅游路线:api/touristRoutes
            links.Add(new LinkDto(Url.Link("GetTouristRoutes", null),
                "get_tourist_routes", "GET"));
            links.Add(new LinkDto(Url.Link("CreateTouristRoute", null),
                "create_tourist_route", "POST"));
            //购物车：api/shoppingCart
            links.Add(new LinkDto(Url.Link("GetShoppingCart", null),
                "get_shopping_cart", "GET"));
            //订单：api/orders
            links.Add(new LinkDto(Url.Link("GetOrders", null),
                "get_orders", "GET"));

            return Ok(links);
        }
    }
}
