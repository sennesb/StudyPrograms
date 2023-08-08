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
           

            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;//如果为false所有的api都会忽略请求的header，返回默认的数据结构json
            }).AddXmlDataContractSerializerFormatters()//新版本实现对xml的支持
            //忽略循环引用
            .AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
             });
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

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