using Autofac;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationFoundation.Interfaces
{
    public interface IServiceBusManager
    {
        Task Publish<T>(object message);
        void StartEsb(ContainerBuilder builder, List<Type> consumers);
    }
}
