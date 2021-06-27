using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.WebApi.Helper;
using System;
using ClientLibrary;
using Products.Core.Interfaces;
using Products.Infrastructure.Services;

namespace Products.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            Activator.CreateInstance<ClientBootstrapper>().Start();
            
            services.ConfigureCustomDbContext()
               .ConfigureCustomMvc()
               .ConfigureCustomCors()
               .ConfigureSwagger()
               .ConfigureCustomAuthentication();

            services.AddAutofac();

            services.AddTransient<IProductService, ProductService>();
            return services.StartContainer();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                //.BuildSeeder(seeder, Configuration)
              .BuildAppDefaults(env)
              .BuildSwaggen();


        }
    }
}
