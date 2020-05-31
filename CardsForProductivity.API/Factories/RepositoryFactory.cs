using CardsForProductivity.API.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CardsForProductivity.API.Factories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        readonly IMongoDatabase _primaryDatabase;
        readonly IMongoDatabase _secondaryDatabase;

        public RepositoryFactory(IOptions<ConnectionStrings> connectionStrings,
            IOptions<StorageOptions> storageOptions)
        {
            var client = new MongoClient(connectionStrings.Value.MongoDB);

            _primaryDatabase = client.GetDatabase(
                storageOptions.Value.DatabaseName, 
                new MongoDatabaseSettings { ReadPreference = ReadPreference.PrimaryPreferred }
            );

            _secondaryDatabase = client.GetDatabase(
                storageOptions.Value.DatabaseName, 
                new MongoDatabaseSettings { ReadPreference = ReadPreference.SecondaryPreferred }
            );
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>(string collectionName, bool readFromPrimary)
        {
            if (readFromPrimary)
                return _primaryDatabase.GetCollection<TEntity>(collectionName);
            else
                return _secondaryDatabase.GetCollection<TEntity>(collectionName);
        }
    }
}
