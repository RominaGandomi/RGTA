using Product.Core.Entities;
using Product.Core.Repositories;


namespace Product.Core.Interfaces
{
    public interface IProductService : IRepository<Products>
    {
    }
}
