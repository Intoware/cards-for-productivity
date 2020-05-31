using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace CardsForProductivity.API.Factories
{
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Gets a Mongo Collection
        /// </summary>
        IMongoCollection<TEntity> GetCollection<TEntity>(string collectionName, bool readFromPrimary);
    }
}