using Products.Core.Entities;
using Products.Core.Interfaces;

namespace Products.Infrastructure.Services
{
    public class ProductService : Repository<Product>, IProductService
    {
        public ProductService(ProductDbContext context) : base(context)
        {
        }
    }
}
