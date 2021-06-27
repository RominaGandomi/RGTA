using MongoDB.Driver;
using Products.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Products.Infrastructure.Interfaces
{
    public interface IProductDbContext
    {
        IMongoCollection<Product> Product { get; set; }
        IMongoCollection<Category> Category { get; set; }
    }
}
