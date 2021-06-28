using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Product.Core.Entities;
using Product.Infrastructure.Interfaces;
using Product.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Infrastructure
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

     
        public IMongoCollection<Products> Product { get; set; }
    }
}
