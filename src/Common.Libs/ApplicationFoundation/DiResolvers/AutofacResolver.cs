using ApplicationFoundation.Interceptors;
using Autofac;
using System;
using Autofac.Extras.DynamicProxy;
using System.Collections.Generic;
using System.Text;

namespace ApplicationFoundation.DiResolvers
{
    internal class AutofacResolver : IContainer
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();
        private Autofac.IContainer _container;

        public T GetBuilder<T>()
        {
            return (T)Convert.ChangeType(_builder, typeof(T));
        }

        public bool IsRegistered(Type type)
        {
            return _container.IsRegistered(type);
        }

        public void Build()
        {
            _container = _builder.Build();
        }

        public void RegisterAsSingle<TT>() where TT : class
        {
            _builder.RegisterType<TT>();
        }

        public void RegisterAsImplementedInterfaces<TT>() where TT : class
        {
            _builder
                .RegisterType<TT>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public void RegisterGeneric(Type type1, Type type2)
        {
            _builder.RegisterGeneric(type1).As(type2).SingleInstance();
        }

        void IContainer.RegisterScoped<TI, TT>()
        {
            _builder.RegisterType<TT>().As<TI>().SingleInstance().InstancePerLifetimeScope();
        }

        void IContainer.RegisterSingleton<TI, TT>()
        {
            _builder.RegisterType<TT>().As<TI>().SingleInstance();
        }

        void IContainer.RegisterTransient<TI, TT>()
        {
            _builder.RegisterType<TT>().As<TI>();
        }

        void IContainer.RegisterWithInterceptor<TI, TT>()
        {
            _builder.RegisterType<TT>().As<TI>().EnableInterfaceInterceptors().InterceptedBy(typeof(AutofacCallHandler));
        }

    }
}
