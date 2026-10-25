using MongoDB.Driver;
using tarea2.Models;

namespace tarea2.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient client)
    {
        _database = client.GetDatabase("tarea2_ventas");
    }

    public IMongoCollection<Venta> Ventas => _database.GetCollection<Venta>("Ventas");
}