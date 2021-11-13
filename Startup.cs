using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Tutorials.Business.Implementation;
using Tutorials.Business.Interface;
using Tutorials.Data.Implementation;
using Tutorials.Data.Interface;

namespace Tutorials
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSingleton<IArticleService, ArticleService>();
            services.AddSingleton<IArticleData, ArticleData>();
            services.AddSingleton<ISectionDetailService, SectionDetailService>();
            services.AddSingleton<ISectionData, SectionData>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddSingleton<TestAbstractClass, AbstractImplementation>();

            //services.AddScoped<ISectionData, SectionData>();
            //services.AddTransient<ISectionData, SectionData>();

            //services.AddMvc().AddApplicationPart(Assembly.Load(new AssemblyName("ClassLibrary")));
            services.AddSingleton<HtmlEncoder>(
     HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin,
                                               UnicodeRanges.CjkUnifiedIdeographs }));


            services.AddTransient<IOperationTransient, Operation>();
            services.AddScoped<IOperationScoped, Operation>();
            services.AddSingleton<IOperationSingleton, Operation>();
            //services.AddSingleton<IOperationSingletonInstance>(new Operation(Guid.Empty));
            ///services.AddTransient<OperationService, OperationService>();
            services.AddSession();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
           Path.Combine(Directory.GetCurrentDirectory(), "Images")),
                RequestPath = "/Images"
            });
            app.UseRouting();
            
            app.UseAuthorization();
            void Test(IApplicationBuilder builder)
            {
            }
            Action<IApplicationBuilder> action = Test;
            //app.Map("/Article", action);
            //app.Map
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/not-found";
                    await next();
                }
                if (context.Response.StatusCode == 403 || context.Response.StatusCode == 503 || context.Response.StatusCode == 500)
                {
                    context.Request.Path = "/Home/Error";
                    await next();
                }
            });
            //app.Run(async (context) => {
            //    context.Response.StatusCode = 200;
            //});
            //app.Run(async (context) =>
            //{
            //    var t = "";
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "MyArea",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}/{id2?}/{desc?}");
            });
        }

        public interface ILoggerManager
        {
            void LogInformation(string message);
            void LogError(string message);
        }

        public class LoggerManager : ILoggerManager
        {
            // Logging functionality happens here
            private readonly ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            public LoggerManager()
            {
                try
                {

                    XmlDocument log4netConfig = new XmlDocument();

                    using (var fs = File.OpenRead("log4net.config"))
                    {
                        log4netConfig.Load(fs);

                        var repo = LogManager.CreateRepository(
                                Assembly.GetEntryAssembly(),
                                typeof(log4net.Repository.Hierarchy.Hierarchy));

                        XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

                        // The first log to be written 
                        _logger.Info("Log System Initialized");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error", ex);
                }
            }

            public void LogInformation(string message)
            {
                _logger.Info(message);
            }

            public void LogError(string message)
            {
                _logger.Error(message);
            }
        }

        public abstract class TestAbstractClass
        {
            public abstract void MethodA();
            public void MethodB()
            {
                //do something
            }
        }

        public class AbstractImplementation : TestAbstractClass
        {
            public override void MethodA()
            {
                //do something
            }
        }


        public interface IOperation
        {
            Guid GetOperationID();
        }

        public interface IOperationTransient : IOperation
        {
        }

        public interface IOperationScoped : IOperation
        {
        }

        public interface IOperationSingleton : IOperation
        {
        }

        public interface IOperationSingletonInstance : IOperation
        {
        }


        public class Operation : IOperationTransient, IOperationScoped, IOperationSingleton, IOperationSingletonInstance
        {
            Guid id;
            public Operation()
            {
                id = Guid.NewGuid();
            }
            public Guid GetOperationID()
            {
                return id;
            }
        }
    }
}
