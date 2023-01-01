using System.IO;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SrsWebApi;

namespace SRSWebApi
{
    /// <summary>
    /// webapi configuration startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// webapi configuration startup class structure
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            /*ThreadPool.GetMinThreads(out var workThreads, out var completionPortThreads);
            Console.WriteLine(new StringBuilder()
                .Append($"ThreadPool.ThreadCount: {ThreadPool.ThreadCount}, ")
                .Append($"Minimum work threads: {workThreads}, ")
                .Append($"Minimum completion port threads: {completionPortThreads})").ToString());
            int maxT; //Maximum number of worker threads
            int maxIO; //Maximum number of IO worker threads
            ThreadPool.GetMaxThreads(out maxT, out maxIO);
            string thMin = string.Format("The default maximum number of worker threads {0}, Maximum number of IO worker threads{1}", maxT, maxIO);
            Console.WriteLine(thMin);
            ThreadPool.SetMinThreads(200, 200); // MinThreads value not to exceed(max_thread /2  )，Otherwise it will not take effect Otherwise, increase the setting max_thread at the same time


            ThreadPool.GetMinThreads(out workThreads, out completionPortThreads);
            Console.WriteLine(new StringBuilder()
                .Append($"ThreadPool.ThreadCount: {ThreadPool.ThreadCount}, ")
                .Append($"Minimum work threads: {workThreads}, ")
                .Append($"Minimum completion port threads: {completionPortThreads})").ToString());*/
        }

        /// <summary>
        /// configuration class
        /// </summary>
        public IConfiguration Configuration { get; }


        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Register Swagger service
            services.AddSwaggerGen(c =>
            {
                // Add document information
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "SRSWebApi", Version = "v1"});
                //c.IncludeXmlComments(Path.Combine(Program.common.WorkPath, "Edu.Model.xml"));//Add model comments here, and the return value will add comments: Edu.Model project properties are required, and the output xml file is generated
                c.IncludeXmlComments(Path.Combine(Program.CommonFunctions.WorkPath, "Edu.Swagger.xml"));
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers().AddJsonOptions(
                options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            ).AddJsonOptions(configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
            });
        }

        /// <summary>
        ///  This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            // Enable Swagger middleware
            app.UseSwagger();

            // Configure SwaggerUI
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "SRSWebApi"); });


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            if (!Directory.Exists(SrsManageCommon.Common.WorkPath + "CutMergeFile"))
            {
                Directory.CreateDirectory(SrsManageCommon.Common.WorkPath + "CutMergeFile");
            }
            
         

            var staticfile = new StaticFileOptions();
            staticfile.FileProvider = new PhysicalFileProvider(SrsManageCommon.Common.WorkPath+"CutMergeFile");//Specify static file server
            //Set the MIME Type manually, or set a default value to solve the problem that the MIME Type file of some files cannot be recognized, and a 404 error occurs
            staticfile.ServeUnknownFileTypes = true;
            staticfile.DefaultContentType = "application/octet-stream";//Set Default MIME Type
            staticfile.OnPrepareResponse = (c) =>
            {
                c.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            };
            app.UseStaticFiles(staticfile);
            app.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });
        }
    }
}