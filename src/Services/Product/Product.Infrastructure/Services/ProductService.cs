using Product.Core.Interfaces;
using Product.Core.Entities;

namespace Product.Infrastructure.Services
{
    public class ProductService : Repository<Products>, IProductService
    {
        public ProductService(ProductDbContext context) : base(context)
        {
        }
    }
}
