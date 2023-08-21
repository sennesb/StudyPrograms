using FakeXiecheng.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

//父类，用来解耦数据验证规则和创建、更新的Dto
//让数据验证规则调用父类实现解耦

namespace FakeXiecheng.API.Dtos
{
    [TouristRouteTitleMustBeDifferentDescriptionAttribute]
    public abstract class TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "title不可为空")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public virtual string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string? Features { get; set; }
        public string? Fees { get; set; }
        public string? Notes { get; set; }
        public double? Rating { get; set; }
        public string? TravelDays { get; set; }
        public string? TripType { get; set; }
        public string? DepartureCity { get; set; }
        //只要变量名一致，automapper都会自动映射，第二第三级更高级的属性也会映射
        public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; } = new List<TouristRoutePictureForCreationDto>();

    }
}
