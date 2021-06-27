using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Products.Core.Entities;
using Products.Infrastructure.Interfaces;
using Products.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Products.Infrastructure
{
    public class ProductDbContext : DbContext
    {
        private IMongoDatabase _db { get; set; }
        public IClientSessionHandle Session { get; set; }
       
        public ProductDbContext(IMongoClient client, string dbName)
        {
            _db = client.GetDatabase(dbName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }

     
        public IMongoCollection<Product> Product { get; set; }
    }
}
