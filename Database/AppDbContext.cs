using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Newtonsoft.Json;

namespace FakeXiecheng.API.Database
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutesPictures { get; set; }

        //可以重写OnModelCreating方法来自定义配置数据模型的细节
        //可以使用ModelBuilder对象来配置实体类、属性、关系和约束等
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder的另外一个用处就是HasData添加初始数据
            /*modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            {
                Id=Guid.NewGuid(),
                Title="ceshititle",
                Description="shuoming",
                OriginalPrice=0,
                CreateTime=DateTime.UtcNow,
                Features = "<p>吃住行游购娱</p>",
                Fees = "<p>交通费用自理</p>",
                Notes = "<p>小心危险</p>"
            });*/

            //通过读取文件来添加初始数据
            //通过反射Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)获取项目当前文件夹地址
            var touristRouteJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            //通过JsonConvert.DeserializeObject方法将JSON数据反序列化为IList<TouristRoute>对象
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

            base.OnModelCreating(modelBuilder);
        }
    }
}
