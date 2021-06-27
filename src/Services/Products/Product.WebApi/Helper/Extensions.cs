using ApplicationFoundation.DiResolvers;
using ApplicationFoundation.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Products.Infrastructure;
using Products.Infrastructure.Data;
using Products.Infrastructure.Interfaces;
using Products.Infrastructure.Model;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Products.WebApi.Helper
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
        public static DateTime CalculateDateTimeFromHour(this int hour)
        {
            return DateTime.Today.AddHours(hour);
        }
        public static string DateToDayString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        public static IEnumerable<DateTime> Range(this DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, (endDate - startDate).Days + 1).Select(d => startDate.AddDays(d));
        }
        public static DateTime StartOfHour(this DateTime hour)
        {
            var dateAsStr = hour.ToString("yyyy MMMM dd HH':'00");
            var date = Convert.ToDateTime(dateAsStr);
            var dateToReturn = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
            return dateToReturn;
        }
        public static DateTime StartOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }
        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }
        public static DateTime StartOfYear(this DateTime date)
        {
            return new DateTime(date.Date.Year, 1, 1);
        }
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Date.Year, date.Date.Month, 1);
        }
    }
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureCustomAuthentication(this IServiceCollection services)
        {
            var authType = ConfigExtension.Instance.GetValue<bool>("AuthenticationType:IdentityServerEnabled");

            if (authType)
            {
                return services.ConfigureIdentityServer();
            }
            else
            {
                return services.ConfigureOktaServer();
            }

        }
        private static IServiceCollection ConfigureIdentityServer(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = ConfigExtension.Instance.GetValue<string>("ApiUrlList:Identity.WebApi");
                options.Audience = "Products";
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                        .GetBytes(ConfigExtension.Instance.GetValue<string>("AppSettings:Token"))),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = "Products",
                    ValidIssuer = ConfigExtension.Instance.GetValue<string>("ApiUrlList:Identity.WebApi")
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModerateRole", policy => policy.RequireRole("Admin", "Moderator"));
                options.AddPolicy("VipOnly", policy => policy.RequireRole("VIP"));
            });

            return services;
        }
        private static IServiceCollection ConfigureOktaServer(this IServiceCollection services)
        {
            // not implemented yet
            return services;
        }
        public static IServiceCollection ConfigureCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });
            return services;
        }
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllParametersInCamelCase();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Products Web Api Swagger Page",
                    Version = "v1",
                    Description = "This tool provides testing environment for Product Web Service",
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                            },
                            new List<string>()
                        }
                    });
            });
            return services;
        }
        public static IServiceCollection ConfigureCustomMvc(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            ;
            services.AddHttpContextAccessor();
            return services;
        }
        public static IServiceCollection ConfigureCustomDbContext(this IServiceCollection services)
        {
            var connectionString = ConfigExtension.Instance.GetValue<string>("Databases:Product:ConnectionString");
            var databaseName = ConfigExtension.Instance.GetValue<string>("Databases:Product:DatabaseName");

            services.AddSingleton<IMongoClient>(s => new MongoClient(connectionString));
            services.AddScoped(s => new ProductDbContext(s.GetRequiredService<IMongoClient>(), databaseName));

            return services;
        }
    }
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder BuildSeeder(this IApplicationBuilder app, IConfiguration configuration)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                //var context = serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>();
                //context.Database.Migrate();

                //var isSeedEnabled = ConfigExtension.Instance.GetValue<bool>("Seed:EnabledForProductApi");
                //if (isSeedEnabled)
                //{
                //    seeder.SeedDefaultTables();
                //}
            }
            return app;
        }
        public static IApplicationBuilder BuildAppDefaults(this IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        context.Response.AddApplicationError(error.Error.Message);
                        await context.Response.WriteAsync(error.Error.Message);
                    }
                });
            });
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCors("CorsPolicy");
            return app;
        }
        public static IApplicationBuilder BuildSwaggen(this IApplicationBuilder app)
        {
            #region Swagger Test Ui Config.

            app.UseSwagger().UseSwaggerUI(sw =>
            {
                sw.DocumentTitle = "Products WebApi  Swagger Page";
                sw.SwaggerEndpoint($"/swagger/v1/swagger.json", "Products.WebApi");
                sw.OAuthClientId("products.swaggerui");
            });

            #endregion
            return app;
        }

    }
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder BuildCustomWebHost(this IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder.UseKestrel()
                 .UseIISIntegration()
                 .UseContentRoot(Directory.GetCurrentDirectory())
                 .ConfigureAppConfiguration((builderContext, config) =>
                 {
                     config.AddEnvironmentVariables();
                 })
                 .ConfigureLogging((context, builder) =>
                 {

                     builder.AddConfiguration(context.Configuration.GetSection("Logging"))
                         .AddConsole()
                         .AddDebug();
                 })
                 .UseStartup<Startup>();
        }
    }
    public static class SessionExtension
    {
        public static ISessionManager Instance
        {
            get
            {
                return DependencyResolver.Instance.Container.Resolve<ISessionManager>();
            }
        }
    }
    public static class ConfigExtension
    {
        public static IConfigManager Instance
        {
            get
            {
                return DependencyResolver.Instance.Container.Resolve<IConfigManager>();
            }
        }
    }
}
