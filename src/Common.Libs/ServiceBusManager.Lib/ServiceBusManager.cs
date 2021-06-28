using ApplicationFoundation.Interfaces;
using Autofac;
using ServiceBusManager.Lib.Helpers;
using ServiceBusManager.Lib.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBusManager.Lib
{
    public class ServiceBusManager : IServiceBusManager
    {
        private static ServiceBusFactory _serviceBusFactory;

        public static ServiceBusFactory ServiceBusFactory
        {
            get
            {
                if (_serviceBusFactory != null) return _serviceBusFactory;
                _serviceBusFactory = (ServiceBusFactory)Activator.CreateInstance(typeof(ServiceBusFactory));
                return _serviceBusFactory;
            }
        }
        public async Task Publish<T>(object message)
        {
            var eventMessage = Extension.ChangeMessageType<IEventMessage>(message);
            await ServiceBusFactory.Publish<T>(eventMessage);
        }
        public void StartEsb(ContainerBuilder builder, List<Type> consumers)
        {
            ServiceBusFactory.StartServiceBus(builder, consumers);
        }

    }
}
