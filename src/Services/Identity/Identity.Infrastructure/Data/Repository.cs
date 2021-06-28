using Identity.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        private bool _disposed = false;

        public Repository(
            DbContext dbContext
            )
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }
        public IQueryable<T> GetAllNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public virtual async Task<ICollection<T>> GetAllAsync()
        {

            return await _dbSet.ToListAsync();
        }
        public virtual T Get(int id)
        {
            return _dbSet.Find(id);
        }
        public virtual async Task<T> GetAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public virtual T Add(T t)
        {
            _dbSet.Add(t);
            _dbContext.SaveChanges();
            return t;
        }
        public virtual async Task<T> AddAsync(T t)
        {
            await _dbSet.AddAsync(t);
            await _dbContext.SaveChangesAsync();
            return t;
        }
        public virtual T Find(Expression<Func<T, bool>> match)
        {
            return _dbSet.SingleOrDefault(match);
        }
        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _dbSet.SingleOrDefaultAsync(match);
        }
        public ICollection<T> FindAll(Expression<Func<T, bool>> match)
        {
            return _dbSet.Where(match).ToList();
        }
        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _dbSet.Where(match).ToListAsync();
        }
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }
        public virtual async void DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        public virtual T Update(T t, object key)
        {
            if (t == null)
                return null;
            var exist = _dbSet.Find(key);
            if (exist != null)
            {
                _dbContext.Entry(exist).CurrentValues.SetValues(t);
                _dbContext.SaveChanges();
            }
            return exist;
        }
        public virtual async Task<T> UpdateAsync(T t, object key)
        {
            if (t == null)
                return null;
            var exist = await _dbSet.FindAsync(key);
            if (exist != null)
            {
                _dbContext.Entry(exist).CurrentValues.SetValues(t);
                await _dbContext.SaveChangesAsync();
            }
            return exist;
        }
        public int Count()
        {
            return _dbSet.Count();
        }
        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
        public virtual void Save()
        {
            _dbContext.SaveChanges();
        }
        public virtual async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            var query = _dbSet.Where(predicate);
            return query;
        }
        public virtual async Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            var queryable = GetAll();
            return includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include<T, object>(includeProperty));
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _dbContext.Dispose();
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
