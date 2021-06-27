using ApplicationFoundation.Interfaces;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Products.Infrastructure.Data;
using Products.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.WebApi.Helper
{
    public static class Bootstrapper
    {
        public static IContainer ApplicationContainer { get; set; }

        public static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<ProductService>().As<ProductService>().InstancePerLifetimeScope();
        }
        public static void ConfigureBrokerContainer(ContainerBuilder builder)
        {
        }
        public static void ConfigureDatabaseContainer(this ContainerBuilder builder)
        {

        }
        public static IServiceProvider StartContainer(this IServiceCollection services)
        {
            #region Building Ioc and start Bus
            var builder = new ContainerBuilder();
            builder.Populate(services);
            ConfigureDatabaseContainer(builder);
            ConfigureContainer(builder);
            ConfigureBrokerContainer(builder);
            ApplicationContainer = builder.Build();


            return new AutofacServiceProvider(ApplicationContainer);

            #endregion
        }
    }
}
