using MongoDB.Driver;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient)
    {
        // Replace 'mydatabase' with your actual database name
        _database = mongoClient.GetDatabase("Cluster0");
    }

    public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
}
