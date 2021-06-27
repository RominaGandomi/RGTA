using ApplicationFoundation.DiResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationFoundation
{
    public class CoreBootstrapper
    {
        public void Start()
        {
            Logging();
            ManageException();
            ConfigureDependency(DependencyResolver.Instance.Container);
        }
        protected virtual void Logging()
        {

        }
        protected virtual void ManageException()
        {

        }
        protected virtual void ConfigureDependency(IContainer container)
        {

        }
        public virtual void Stop()
        {

        }
    }
}
