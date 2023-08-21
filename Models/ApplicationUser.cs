using Microsoft.AspNetCore.Identity;

//在IdentityUser基础上拓展用户模型

namespace FakeXiecheng.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public ICollection<Order> Order { get; set; }

        //使用virtual重载以下四张表，建立数据模型的关系
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }//用户的角色
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }//用户权限
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }//用户第三方登录信息
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
    }
}
