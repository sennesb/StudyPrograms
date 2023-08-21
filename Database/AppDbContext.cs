using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace FakeXiecheng.API.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutesPictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Order> Orders { get; set; }

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


            //添加初始化用户数据
            //1.关系用户与角色的外键
            modelBuilder.Entity<ApplicationUser>(u =>
            u.HasMany(x => x.UserRoles)
            .WithOne().HasForeignKey(ur => ur.UserId).IsRequired());
            //2.添加管理员角色
            var adminRoleId = "308660dc-ae51-480f-824d-7dca6714c3e2";
            modelBuilder.Entity<IdentityRole>()
                .HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                });
            //2.添加用户
            var adminUserId = "90184155-dee0-40c9-bb1e-b5ed07afc04e";
            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@fakexiecheng.com",
                NormalizedUserName = "admin@fakexiecheng.com".ToUpper(),
                Email = "admin@fakexiecheng.com",
                NormalizedEmail = "admin@fakexiecheng.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = false,
                Address = "jia"
            };
              //把明文密码hash后再保存进数据库
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Fake123$");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);
            //4.给用户加入管理员角色
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> 
                { 
                    RoleId = adminRoleId,
                    UserId = adminUserId,
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}
