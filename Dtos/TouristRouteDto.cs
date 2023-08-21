using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

//数据输出Dto

namespace FakeXiecheng.API.Dtos
{
    public class TouristRouteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //
        public decimal Price { get; set; }//计算方式：原价 * 折扣
        public decimal OriginalPrice { get; set; }
        public double? DiscountPresent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string Features { get; set; }
        public string Fees { get; set; }
        public string Notes { get; set; }
        public double? Rating { get; set; }
        public string TravelDays { get; set; }
        public string TripType { get; set; }
        public string DepartureCity { get; set; }
        //当两个对象的某个字段完全一致时，automapper会自动做数据的映射
        //而字段的类型是在profile中注册过的对象时，不用做任何配置，automapper会接管所有的映射
        public ICollection<TouristRoutePictureDto> TouristRoutePictures { get; set; }
    }
}
