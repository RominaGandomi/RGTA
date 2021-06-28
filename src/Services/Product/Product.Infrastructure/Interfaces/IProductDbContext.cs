using MongoDB.Driver;
using Product.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Infrastructure.Interfaces
{
    public interface IProductDbContext
    {
        IMongoCollection<Products> Product { get; set; }
        IMongoCollection<Category> Category { get; set; }
    }
}
