using Autofac;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Hosting;
using ServiceBusManager.Lib.Helpers;
using ServiceBusManager.Lib.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.RabbitMqTransport;
using System.Linq;

namespace ServiceBusManager.Lib
{
    public class ServiceBusFactory : IHostedService
    {
        private static IBusControl _busControl;
        private static IBus Bus => _busControl;

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return _busControl.StartAsync(cancellationToken);
        }
        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return _busControl.StopAsync(cancellationToken);
        }
        public void StartServiceBus(ContainerBuilder builder, List<Type> consumers = null)
        {
            builder.AddMassTransit(x =>
            {
                if (consumers?.Any() == true)
                {
                    consumers.ForEach(item => x.AddConsumer(item));
                }
                x.AddBus(ConfigureBus);
            });
        }
        private IBusControl ConfigureBus(IComponentContext context)
        {

            _busControl = MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var connStr = ConfigExtension.Instance.GetValue<string>("ServiceBus.Server:ServiceBusConnection");

                var host = cfg.Host(new Uri(connStr), "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint(host, "Engie.ApiPortal-MQ-EndPoint", ec =>
                {
                    ec.PrefetchCount = 16;
                    ec.UseMessageRetry(r => r.Interval(2, 100));
                    ec.ConfigureConsumers(context);
                });
                cfg.ConfigureEndpoints(context);
            });
            StartAsync();
            return _busControl;

        }

        public static async Task Publish<T>(IEventMessage message)
        {
            if (!string.IsNullOrEmpty(message.Data))
            {
                if (message is T)
                {
                    await Bus.Publish(message);
                }
            }
        }
    }
}
