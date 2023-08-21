using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FakeXiecheng.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITouristRouteRepository _touristRouteRepository;

        //构建时注入使用配置文件、UserManager、signInManager的依赖
        public AuthenticateController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITouristRouteRepository touristRouteRepository)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _touristRouteRepository = touristRouteRepository;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginDto loginDto)
        {
            //1.使用signInManager验证用户密码
            var loginResult = await _signInManager.PasswordSignInAsync(
                loginDto.Email, loginDto.Password,false,false);
            if (!loginResult.Succeeded)
            {
                return BadRequest();
            }
              //验证成功后从数据库获取用户具体信息
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            //2.创建jwt token 
             //header
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
             //payload
            var claims = new List<Claim>()
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Id),//用户id（sub）
                 //new Claim(ClaimTypes.Role, "Admin")
             };
            //获取用户角色
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach(var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }

            //signiture
            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);//读取配置文件私钥
            var signingKey = new SymmetricSecurityKey(secretByte);//使用非对称算法进行加密
            var signingCredentials = new SigningCredentials(signingKey, signingAlgorithm);//使用hs256验证加密后的私钥
            //创建token
            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials);
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            //3.return 200ok + 添加jwt
            return Ok(tokenStr);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            //1.使用用户名创建用户对象
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };
            //2.hush用户密码，和用户对象一起保存到数据库，创建jwt token
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            //3.初始化购物车
            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };
            await _touristRouteRepository.CreateShoppingCart(shoppingCart);
            await _touristRouteRepository.SaveAsync();

            //4.返回
            return Ok();
        }
    }
}
