using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FakeXiecheng.API.Database;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Data;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using FakeXiecheng.API.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FakeXiecheng.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //注册身份认证服务
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            //注册身份验证服务
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Authentication:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration["Authentication:Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretByte)
                    };
                });


            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;//如果为false所有的api都会忽略请求的header，返回默认的数据结构json
            })
                //注册Newtonsoft.Json作为默认的JSON序列化器、以便支持System.Text.Json不支持的特殊数据类型的JSON转换，比如JsonPatchDocument。
                //setupActio里设置目的是将JSON序列化时的属性名转换为小驼峰命名法（camel case），即首字母小写，单词间用驼峰形式连接。
                .AddNewtonsoftJson(setupAction => { setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); })

                //新版本实现对xml的支持
                .AddXmlDataContractSerializerFormatters()
               
                //忽略循环引用
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                })

                //配置非法模型状态响应工厂，验证数据是否非法
                //ConfigureApiBehaviorOptions方法可以自定义API的行为和响应结果
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    //通过设置InvalidModelStateResponseFactory属性，可以实现自定义的模型验证失败处理逻辑
                    //在数据验证不成功时把默认返回的400改成返回422
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        //ModelState用于存储模型验证的结果，包含了每个属性的验证状态和错误消息。
                        //ValidationProblemDetails对象可以用于生成包含验证问题详细信息的响应结果。提供了一些属性，如Type、Title、Status、Detail等，用于描述验证失败的情况，并可以添加额外的扩展属性。
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "无所谓",
                            Title = "数据验证失败",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "请看详细说明",
                            Instance = context.HttpContext.Request.Path
                        };
                        problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                })
            ;


            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
            services.AddDbContext<AppDbContext>(options => 
            {
                //options.UseSqlServer("server=localhost; Database=FakeXiechengDb; User Id=SA; Password=PaSSword12!;");
                options.UseSqlServer(Configuration["DbContext:ConnectionString"]);
                //options.UseMySql(Configuration["DbContext:MysSQLConnectionString"],new MySqlServerVersion(new Version(8,0,27)));
            }) ;
            //AutoMapper会自动扫描程序集所有包含映射关系的proflie文件，调用以下方法把profile文件加载到目前的AppDomain中
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //添加发起HTTP请求的服务依赖
            services.AddHttpClient();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            //注册全局添加自定义媒体类型的服务依赖
            services.Configure<MvcOptions>(config =>
            {
                var outputFormater = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                if (outputFormater != null)
                {
                    outputFormater.SupportedMediaTypes.Add("application/vnd.api.hatoeas+json");
                }
            });


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //设置默认首页
            FileServerOptions fileServerOptions = new FileServerOptions();
            fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseFileServer(fileServerOptions);

            app.UseRouting();//
            app.UseAuthentication();//启动身份验证
            app.UseAuthorization();//确认权限

            app.UseEndpoints(endpoints =>
            {
                /*                endpoints.MapGet("/test", async context =>
                                {
                                    throw new Exception("test");
                                    //await context.Response.WriteAsync("Hello from test!");
                                });

                                endpoints.MapGet("/", async context =>
                                {
                                    await context.Response.WriteAsync("Hello World!");
                                });*/

                endpoints.MapControllers();
            });
        }
    }
}