using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SrsApis.SrsManager;
using SRSWebApi;
using System.Text.Json.Serialization;

namespace SRSManager.Tests.Fixtures
{
    public class ApiWebApplicationFactory : WebApplicationFactory<List<SrsManager>>
    {
        public IConfiguration Configuration { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                Configuration = new ConfigurationBuilder()
                  .AddJsonFile("integrationsettings.json")
                  .Build();

                config.AddConfiguration(Configuration);
            });

            builder.ConfigureTestServices(services =>
            {
                // Register Swagger service
                services.AddSwaggerGen(c =>
                {
                    // Add document information
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SRSWebApi", Version = "v1" });
                    //c.IncludeXmlComments(Path.Combine(Program.common.WorkPath, "Edu.Model.xml"));//Add model comments here, and the return value will add comments: Edu.Model project properties are required, and the output xml file is generated
                    c.IncludeXmlComments(Path.Combine(WebApis.CommonFunctions.WorkPath, "Edu.Swagger.xml"));
                });
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                services.AddControllers().AddJsonOptions(
                    options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
                ).AddJsonOptions(configure =>
                {
                    configure.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                });
            });
        }
    }
}
