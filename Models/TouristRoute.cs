using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeXiecheng.API.Models
{
    public class TouristRoute
    {
        [Key]//主键
        public Guid Id { get; set; }
        [Required]//不能为空
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(150)]
        public string Description { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OriginalPrice { get; set; }
        [Range(0.0, 1.0)]
        public double? DiscountPresent { get; set; }//?代表可空
        public DateTime CreateTime { get; set; }
        public DateTime? UpdatedTime { get; set;}
        public DateTime? DepartureTime { get; set; }
        [MaxLength]//最大长度
        public string Features { get; set; }
        [MaxLength]
        public string Fees { get; set; }
        [MaxLength]
        public string Notes { get; set; }
        public ICollection<TouristRoutePicture> TouristRoutePictures { get; set; } 
            = new List<TouristRoutePicture>();//外键关系
        public double? Rating { get; set; }
        public TravelDays? TravelDays { get; set; }
        public TripType? TripType { get; set; }
        public DepartureCity? DepartureCity { get; set;}
    }
}
    