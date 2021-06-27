using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Products.Core.Repositories;
using Products.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Products.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        internal readonly ProductDbContext _dbContext;
        private readonly IMongoCollection<T> _dbCollection;

        public Repository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbCollection = _dbContext.GetCollection<T>(typeof(T).Name);
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }
        public virtual IQueryable<T> AsQueryable()
        {
            return _dbCollection.AsQueryable();
        }
        public virtual IEnumerable<T> FilterBy(
            Expression<Func<T, bool>> filterExpression)
        {
            return _dbCollection.Find(filterExpression).ToEnumerable();
        }

        public virtual IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<T, bool>> filterExpression,
            Expression<Func<T, TProjected>> projectionExpression)
        {
            return _dbCollection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        public virtual T FindOne(Expression<Func<T, bool>> filterExpression)
        {
            return _dbCollection.Find(filterExpression).FirstOrDefault();
        }

        public virtual Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression)
        {
            return Task.Run(() => _dbCollection.Find(filterExpression).FirstOrDefaultAsync());
        }

        public virtual T FindById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            return _dbCollection.Find(filter).SingleOrDefault();
        }
       


        public virtual List<T> GetAll()
        {
            var all =  _dbCollection.Find(Builders<T>.Filter.Empty);
            return  all.ToList();
        }
        public virtual Task<T> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<T>.Filter.Eq("_id", objectId);
                return _dbCollection.Find(filter).SingleOrDefaultAsync();
            });
        }


        public virtual void InsertOne(T document)
        {
            _dbCollection.InsertOne(document);
        }

        public virtual Task InsertOneAsync(T document)
        {
            return Task.Run(() => _dbCollection.InsertOneAsync(document));
        }

        public void InsertMany(ICollection<T> documents)
        {
            _dbCollection.InsertMany(documents);
        }


        public virtual async Task InsertManyAsync(ICollection<T> documents)
        {
            await _dbCollection.InsertManyAsync(documents);
        }

        public void DeleteOne(Expression<Func<T, bool>> filterExpression)
        {
            _dbCollection.FindOneAndDelete(filterExpression);
        }

        public Task DeleteOneAsync(Expression<Func<T, bool>> filterExpression)
        {
            return Task.Run(() => _dbCollection.FindOneAndDeleteAsync(filterExpression));
        }

        public void DeleteById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            _dbCollection.FindOneAndDelete(filter);
        }

        public Task DeleteByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<T>.Filter.Eq("_id", objectId);
                _dbCollection.FindOneAndDeleteAsync(filter);
            });
        }

        public void DeleteMany(Expression<Func<T, bool>> filterExpression)
        {
            _dbCollection.DeleteMany(filterExpression);
        }

        public Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression)
        {
            return Task.Run(() => _dbCollection.DeleteManyAsync(filterExpression));
        }

    }
}
