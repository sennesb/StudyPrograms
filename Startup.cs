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
            //ע�������֤����
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            //ע�������֤����
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
                setupAction.ReturnHttpNotAcceptable = true;//���Ϊfalse���е�api������������header������Ĭ�ϵ����ݽṹjson
            })
                //ע��Newtonsoft.Json��ΪĬ�ϵ�JSON���л������Ա�֧��System.Text.Json��֧�ֵ������������͵�JSONת��������JsonPatchDocument��
                //setupActio������Ŀ���ǽ�JSON���л�ʱ��������ת��ΪС�շ���������camel case����������ĸСд�����ʼ����շ���ʽ���ӡ�
                .AddNewtonsoftJson(setupAction => { setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); })

                //�°汾ʵ�ֶ�xml��֧��
                .AddXmlDataContractSerializerFormatters()
               
                //����ѭ������
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                })

                //���÷Ƿ�ģ��״̬��Ӧ��������֤�����Ƿ�Ƿ�
                //ConfigureApiBehaviorOptions���������Զ���API����Ϊ����Ӧ���
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    //ͨ������InvalidModelStateResponseFactory���ԣ�����ʵ���Զ����ģ����֤ʧ�ܴ����߼�
                    //��������֤���ɹ�ʱ��Ĭ�Ϸ��ص�400�ĳɷ���422
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        //ModelState���ڴ洢ģ����֤�Ľ����������ÿ�����Ե���֤״̬�ʹ�����Ϣ��
                        //ValidationProblemDetails��������������ɰ�����֤������ϸ��Ϣ����Ӧ������ṩ��һЩ���ԣ���Type��Title��Status��Detail�ȣ�����������֤ʧ�ܵ��������������Ӷ������չ���ԡ�
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "����ν",
                            Title = "������֤ʧ��",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "�뿴��ϸ˵��",
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
            //AutoMapper���Զ�ɨ��������а���ӳ���ϵ��proflie�ļ����������·�����profile�ļ����ص�Ŀǰ��AppDomain��
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //��ӷ���HTTP����ķ�������
            services.AddHttpClient();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            //ע��ȫ������Զ���ý�����͵ķ�������
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

            //����Ĭ����ҳ
            FileServerOptions fileServerOptions = new FileServerOptions();
            fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseFileServer(fileServerOptions);

            app.UseRouting();//
            app.UseAuthentication();//���������֤
            app.UseAuthorization();//ȷ��Ȩ��

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