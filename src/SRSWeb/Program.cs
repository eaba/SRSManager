
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Akka.Hosting;
using Akka.Actor;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using SRSWeb.Actors;
using SharpPulsar;

namespace SRSWeb;
/// <summary>
/// Generic class for webapi
/// </summary>

public class Program
{
    public static CommonFunctions CommonFunctions = new CommonFunctions();
    public static void Main(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAkka("SRSManager", configurationBuilder =>
        {
           
            configurationBuilder.WithActors((system, registry) =>
            {
                //v2.11.0 
                //var s = PulsarSystem.GetInstanceAsync(system, null, actorSystemName: "tests");
                var global = system.ActorOf(GlobalSrsApisActor.Prop(), "GlobalSrsApis");
                registry.TryRegister<GlobalSrsApisActor>(global); // register for DI
            });
        });
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        // Register Swagger service
        builder.Services.AddSwaggerGen(c =>
        {
            // Add document information
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SRSWebApi", Version = "v1" });
            //c.IncludeXmlComments(Path.Combine(Program.common.WorkPath, "Edu.Model.xml"));//Add model comments here, and the return value will add comments: Edu.Model project properties are required, and the output xml file is generated
            c.IncludeXmlComments(Path.Combine(Program.CommonFunctions.WorkPath, "Edu.Swagger.xml"));
        });
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddControllers().AddJsonOptions(
            options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
        )
        .AddJsonOptions(configure =>
        {
            configure.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
        });
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
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
        staticfile.FileProvider = new PhysicalFileProvider(SrsManageCommon.Common.WorkPath + "CutMergeFile");//Specify static file server
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
        
        app.Run();


    }
}