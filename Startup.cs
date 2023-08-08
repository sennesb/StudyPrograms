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
                setupAction.ReturnHttpNotAcceptable = true;//���Ϊfalse���е�api������������header������Ĭ�ϵ����ݽṹjson
            }).AddXmlDataContractSerializerFormatters()//�°汾ʵ�ֶ�xml��֧��
            //����ѭ������
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
            //AutoMapper���Զ�ɨ��������а���ӳ���ϵ��proflie�ļ����������·�����profile�ļ����ص�Ŀǰ��AppDomain��
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