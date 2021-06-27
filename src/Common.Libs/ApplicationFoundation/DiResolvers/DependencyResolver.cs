using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ApplicationFoundation.DiResolvers
{
    public class DependencyResolver
    {
        private DependencyResolver()
        {

        }
        private static DependencyResolver _instance;
        private static readonly object Locker = new object();
        public IContainer Container = new AutofacResolver();
        public static DependencyResolver Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                lock (Locker)
                {
                    return _instance ?? (_instance = new DependencyResolver());
                }
                
            }
        }

        
        
    }
}
