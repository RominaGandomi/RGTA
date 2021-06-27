using ApplicationFoundation.DiResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationFoundation
{
    public interface IContainer
    {
        void RegisterSingleton<TI, TT>() where TI : class
                                 where TT : class, TI;

        void RegisterScoped<TI, TT>() where TI : class
                                 where TT : class, TI;
        void RegisterTransient<TI, TT>() where TI : class
                               where TT : class, TI;

        void RegisterWithInterceptor<TI, TT>() where TI : class where TT : class, TI;
        void RegisterGeneric(Type type1, Type type2);
        void RegisterAsSingle<TT>() where TT : class;
        void RegisterAsImplementedInterfaces<TT>() where TT : class;
        T Resolve<T>();
        T GetBuilder<T>();
        bool IsRegistered(Type type);
        void Build();

        public static IContainer GetContainer()
        {
            return new AutofacResolver();
        }
    }
}
