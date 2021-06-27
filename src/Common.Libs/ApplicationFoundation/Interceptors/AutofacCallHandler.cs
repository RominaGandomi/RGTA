using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace ApplicationFoundation.Interceptors
{
    internal class AutofacCallHandler : IInterceptor
    {

        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"{invocation.Method.Name} method invoked");

            using (var transaction = new TransactionScope())
            {
                try
                {
                    invocation.Proceed();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error has occured: {ex.Message}");
                }
            }

            Console.WriteLine($"{invocation.Method.Name} method finished");
        }
    }
}
