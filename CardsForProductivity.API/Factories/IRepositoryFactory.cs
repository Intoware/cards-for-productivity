using MongoDB.Driver;

namespace CardsForProductivity.API.Factories
{
    /// <summary>
    /// Repository factory interface.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Gets a Mongo collection.
        /// </summary>
        /// <param name="collectionName">Name of the collection to get.</param>
        /// <param name="readFromPrimary">Whether records should be read from the primary collection.</param>
        /// <typeparam name="TEntity">Type of the entity contained within the collection.</typeparam>
        /// <returns>Mongo Collection of type TEntity.</returns>
        IMongoCollection<TEntity> GetCollection<TEntity>(string collectionName, bool readFromPrimary);
    }
}