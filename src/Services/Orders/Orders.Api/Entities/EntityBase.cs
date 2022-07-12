using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ToyTrucks.Orders.Api.Entities
{
    public abstract class EntityBase
    {
        // [BsonId]
        // [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}