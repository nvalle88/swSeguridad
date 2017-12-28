using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace bd.swseguridad.web
{
    public class Program
    {
        /// <summary>
        /// Se inicializa Asp.Net Core
        /// Para más información visitar:https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?tabs=aspnetcore2x
        /// </summary>
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
