using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IdentityDbContext>();
            const string connectionString = "Server=localhost,1420; Database=Rgta.Identity; User ID=sa; Password=Gem2019*;";
            
            builder.UseSqlServer(connectionString);

            return new IdentityDbContext(builder.Options);
        }
    }
}
