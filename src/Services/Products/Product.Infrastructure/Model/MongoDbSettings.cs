using System;
using System.Collections.Generic;
using System.Text;

namespace Products.Infrastructure.Model
{
    public class MongoDbSettings 
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
