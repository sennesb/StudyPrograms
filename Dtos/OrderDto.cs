using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ICollection<LineItemDto> OrderItems { get; set; }
        public string State { get; set; }//订单目前状态
        public DateTime CreateDateUTC { get; set; }
        public string TransactionMetadata { get; set; }
    }
}
