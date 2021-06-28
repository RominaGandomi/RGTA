using ApplicationFoundation.DiResolvers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Identity.Core.Repositories;
using Identity.Core.Services;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Services;
using Identity.Infrastructure.Uow;
using Identity.WebApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.Helpers
{
    public static class Bootstrapper
    {
        public static Autofac.IContainer ApplicationContainer { get; set; }

        public static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<IUrlHelper>();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().SingleInstance();
            builder.RegisterType<Seed>();
            builder.RegisterType<DbContext>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthHelper>().As<IAuthHelper>().InstancePerLifetimeScope();
            builder.RegisterType<TenantService>().As<ITenantService>().InstancePerLifetimeScope();
        }
        public static void ConfigureBrokerContainer(ContainerBuilder builder)
        {
            //builder.RegisterType<UserActionsService>().As<IUserActionsService>().InstancePerLifetimeScope();
            //builder.RegisterType<UserActionsEventConsumer>().InstancePerLifetimeScope();

            //var bus = DependencyResolver.Instance.Container.Resolve<IServiceBusManager>();

            //var typeList = new List<Type>
            //{
            //    typeof(UserActionsEventConsumer),
            //};

            //bus.StartEsb(builder, typeList);

        }
        public static void ConfigureDatabaseContainer(this ContainerBuilder builder)
        {
            builder
              .RegisterType<IdentityDbContext>()
              .As<DbContext>()
              .InstancePerLifetimeScope();

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
            //ApplicationContainer.Resolve<IBusControl>().StartAsync();

            return new AutofacServiceProvider(ApplicationContainer);

            #endregion
        }
    }
}
