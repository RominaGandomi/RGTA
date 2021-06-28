using Identity.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        bool BeginNewTransaction();
        bool RollBackTransaction();
        int Commit();
    }
}
