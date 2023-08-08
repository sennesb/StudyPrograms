using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeXiecheng.API.Models
{
    public class TouristRoutePicture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增类型
        public int Id { get; set; }
        [MaxLength(100)]
        public string Url { get; set; }
        [ForeignKey("TouristRouteId")]//EF在映射数据库时会自动把每个模型的主键以类目加上Id的形式做外键关联
        public Guid TouristRouteId { get; set; }//与旅游路线的外键关系
        public TouristRoute TouristRoute { get; set; }//连接属性，如果想要获取该图片所属的旅游路线的属性，可以使用touristRoutePicture.TouristRoute.属性名来访问
    }
}
