

namespace Product.Infrastructure.Data
{
    public class Seed
    {
        private readonly ProductDbContext _context;
        public Seed(ProductDbContext context)
        {
            _context = context;
        }
        public void SeedDefaultTables()
        {

        }
    }
}
