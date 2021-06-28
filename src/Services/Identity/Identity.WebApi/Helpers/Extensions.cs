using ApplicationFoundation.DiResolvers;
using ApplicationFoundation.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Identity.WebApi.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
        {
            return services.ConfigureIdentityServer();
        }
        private static IServiceCollection ConfigureIdentityServer(this IServiceCollection services)
        {

            var builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<IdentityDbContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
            builder.AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Authority = ConfigExtension.Instance.GetValue<string>("ApiUrlList:Identity.WebApi");
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(ConfigExtension.Instance.GetValue<string>("AppSettings:Token"))),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidIssuer = ConfigExtension.Instance.GetValue<string>("ApiUrlList:Identity.WebApi")

                    };

                    options.Configuration = new OpenIdConnectConfiguration();
                })
            .AddIdentityCookies();

            var identityResources = new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                   .GetBytes(ConfigExtension.Instance.GetValue<string>("AppSettings:Token")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);



            services.AddIdentityServer()
             .AddDeveloperSigningCredential()

             .AddInMemoryPersistedGrants()
             .AddInMemoryIdentityResources(identityResources)
             .AddAspNetIdentity<User>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModerateRole", policy => policy.RequireRole("Admin", "Moderator"));
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

            services.AddControllersWithViews(options =>
            {
                options.EnableEndpointRouting = false;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddHttpContextAccessor();
            services.AddSession(o =>
            {
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.Name = "Identity.Session";
                o.Cookie.HttpOnly = true;
            });
            services.AddSignalR();

            return services;
        }
        public static IServiceCollection ConfigureCustomDbContext(this IServiceCollection services)
        {
            var connectionString = ConfigExtension.Instance.GetValue<string>("Databases:Identity:ConnectionString");
            services.AddDbContext<IdentityDbContext>(x =>
            {
                x.UseSqlServer(connectionString);
            },
                ServiceLifetime.Transient
            );
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });
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
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RGTA.Identity Web Api Swagger Page",
                    Version = "v1",
                    Description = "This tool provides testing environment for RGTA Identity Web Service",
                });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
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
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
            });
            return services;
        }
    }

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder BuildSeeder(this IApplicationBuilder app, Seed seeder)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                context.Database.Migrate();

                var isSeedEnabled = ConfigExtension.Instance.GetValue<bool>("Seed:EnabledForMainApi");
                if (isSeedEnabled)
                {
                    seeder.SeedUsers();
                    seeder.SeedTenant();
                }
            }
            return app;
        }
        public static IApplicationBuilder BuildAppDefaults(this IApplicationBuilder app, IWebHostEnvironment env)
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


            #region SignalR Declarations
            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<LoginHub>("/auth/login");
            //});

            #endregion


            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseIdentityServer();
            app.UseCookiePolicy();
            app.UseMiddleware<TenantProviderMiddleware>();
            app.UseAuthentication();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            return app;
        }
        public static IApplicationBuilder BuildSwaggen(this IApplicationBuilder app)
        {
            #region Swagger Test Ui Config.

            app.UseSwagger().UseSwaggerUI(sw =>
            {
                sw.DocumentTitle = "Identity Swagger Page";
                sw.SwaggerEndpoint($"/swagger/v0/swagger.json", "Identity.WebApi");
                sw.OAuthClientId("identity.swaggerui");
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
