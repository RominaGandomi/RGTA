using ApplicationFoundation;
using ApplicationFoundation.Interfaces;
using ServiceBusManager.Lib;
using System;


namespace ClientLibrary
{
    public class ClientBootstrapper : CoreBootstrapper
    {
        private IContainer _container;
        protected override void ConfigureDependency(IContainer container)
        {
            base.ConfigureDependency(container);
            _container = container;
            container.RegisterScoped<IExceptionManager, ExceptionManager.Lib.ExceptionManager>();
            container.RegisterScoped<IConfigManager, ConfigManager.Lib.ConfigManager>();
            container.RegisterScoped<IServiceBusManager, ServiceBusManager.Lib.ServiceBusManager>();
            container.RegisterScoped<ISessionManager, SessionManager.Lib.SessionManager>();
            container.Build();
        }

        protected override void ManageException()
        {
            base.ManageException();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _container.Resolve<IExceptionManager>().Handle(e.ExceptionObject as Exception);
        }
    }
}
