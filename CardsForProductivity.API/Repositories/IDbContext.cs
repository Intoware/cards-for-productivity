using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;
using MongoDB.Driver;

namespace CardsForProductivity.API.Repositories
{
    /// <summary>
    /// Database context.
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// Session model collection.
        /// </summary>
        /// <value>Mongo collection containing SessionModels.</value>
        IMongoCollection<SessionModel> SessionModels { get; }

        /// <summary>
        /// Story model collection.
        /// </summary>
        /// <value>Mongo collection containing StoryModels.</value>
        IMongoCollection<StoryModel> StoryModels { get; }

        /// <summary>
        /// User model collection.
        /// </summary>
        /// <value>Mongo collection containing UserModels.</value>
        IMongoCollection<UserModel> UserModels { get; }

        /// <summary>
        /// Creates the indexes for each collection.
        /// </summary>
        Task SetupAsync(CancellationToken token);
    }
}