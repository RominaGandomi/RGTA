using Identity.Core.Repositories;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Identity.Infrastructure.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityDbContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        public UnitOfWork(IdentityDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("dbContext can not be null.");
            try
            {
                _context = context;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }
        public bool BeginNewTransaction()
        {
            try
            {
                _transaction = _context.Database.BeginTransaction();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool RollBackTransaction()
        {
            try
            {
                _transaction.Rollback();
                _transaction = null;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public int Commit()
        {
            var transaction = _transaction ?? _context.Database.BeginTransaction();
            using (transaction)
            {
                try
                {
                    if (_context == null)
                    {
                        throw new ArgumentException("Context is null");
                    }
                    var result = _context.SaveChanges();

                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error on save changes ", ex);
                }
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
