using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Products.WebApi.Helper;

namespace Products.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHostBuilder = CreateWebHostBuilder(args);
            webHostBuilder.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .BuildCustomWebHost();

        
    }
}
