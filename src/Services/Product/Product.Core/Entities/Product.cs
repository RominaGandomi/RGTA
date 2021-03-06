using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Product.Core.Entities
{
    public class Products
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageFilePath { get; set; }
        public decimal Price { get; set; }
    }
}
