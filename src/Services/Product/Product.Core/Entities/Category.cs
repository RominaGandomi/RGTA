using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Product.Core.Entities
{
    public class Category
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }

    }
}
