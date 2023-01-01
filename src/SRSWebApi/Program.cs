using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SRSWebApi;

namespace SrsWebApi
{
    /// <summary>
    /// Entry class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Generic class for webapi
        /// </summary>
        public static CommonFunctions CommonFunctions = new CommonFunctions();

        /// <summary>
        /// program entry
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CommonFunctions.CommonInit();
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// create httpwebserver
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls(CommonFunctions.BaseUrl);
                });
    }
}