using MongoDB.Driver;
using ToyTrucks.Orders.Api.Entities;
using Microsoft.Extensions.Configuration;
namespace ToyTrucks.Orders.Api.DBContexts
{
    public class OrderContext : IOrderContext
    {

        public OrderContext(IConfiguration configuration)
        {
            var server = configuration["MONGO_SERVER"];// ?? "localhost";

            var port = configuration["MONGO_PORT"];// ?? "5432";
            var database = configuration["MONGO_DB"] ?? "orderdb";


            var collection = configuration["MONGO_COLLECTION"];// ?? "marcas";
            var client = new MongoClient($"mongodb://{server}:{port}");

            var db = client.GetDatabase(database);
            Orders = db.GetCollection<Order>(collection);

            //   OrderSeedData.SeedData(Orders);

            //docker run -d -p 27017:27017 --name order-mongo   mongo
            //following are mongo cmds
            //mongo
            //show dbs
            //show collections
            //use OrderDb //creates anew db
            //db.createCollection('Orders')
            //db.Orders.insertMany([{"name","value", "name2","value2"}])
            //db.Orders.remove({});
            //db.Orders.find({}).pretty();

            //DONT go into collections
        }
        public IMongoCollection<Order> Orders { get; }
    }
}