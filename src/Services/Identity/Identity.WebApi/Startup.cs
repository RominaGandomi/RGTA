using Autofac.Extensions.DependencyInjection;
using ClientLibrary;
using Identity.Infrastructure.Data;
using Identity.WebApi.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Identity.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Activator.CreateInstance<ClientBootstrapper>().Start();
            services.ConfigureCustomDbContext()
                .ConfigureCustomMvc()
                .ConfigureCustomCors()
                .ConfigureSwagger()
                .ConfigureAuthentication();

            services.AddAutofac();
            return services.StartContainer();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Seed seeder)
        {
            app.BuildAppDefaults(env)
                .BuildSeeder(seeder)
                .BuildSwaggen();
        }
    }
}
