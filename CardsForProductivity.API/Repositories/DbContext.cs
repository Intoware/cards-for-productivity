using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Factories;
using CardsForProductivity.API.Models.Data;
using MongoDB.Driver;

namespace CardsForProductivity.API.Repositories
{
    public class DbContext : IDbContext
    {
        private readonly IRepositoryFactory _repoFactory;

        public DbContext(IRepositoryFactory repoFactory)
        {
            SessionModels = repoFactory.GetCollection<SessionModel>("Sessions", true);
            StoryModels = repoFactory.GetCollection<StoryModel>("Stories", true);
            UserModels = repoFactory.GetCollection<UserModel>("Users", true);
            
            _repoFactory = repoFactory;
        }

        public IMongoCollection<SessionModel> SessionModels { get; }

        public IMongoCollection<StoryModel> StoryModels { get; }

        public IMongoCollection<UserModel> UserModels { get; }

        public async Task SetupAsync(CancellationToken token)
        {
            await InitialiseCollectionsAsync(StoryModels, token);
            await CreateIndexesAsync(token);
        }

        private async Task InitialiseCollectionsAsync<T>(IMongoCollection<T> collection, CancellationToken token)
        {
            var existing = await collection.Database.ListCollectionNames().ToListAsync(token);
            
            if (!existing.Any(x => x == collection.CollectionNamespace.CollectionName))
            {
                await collection.Database.CreateCollectionAsync(collection.CollectionNamespace.CollectionName, null, token);
            }
        }

        private async Task CreateIndexesAsync(CancellationToken cancellationToken)
        {
            var sessionCollectionIndexKeys = Builders<SessionModel>.IndexKeys;
            var sessionCollectionIndexModel = new CreateIndexModel<SessionModel>(
                sessionCollectionIndexKeys.Ascending(i => i.SessionCode).Ascending(i => i.SessionCode)
            );
            await SessionModels.Indexes.CreateOneAsync(sessionCollectionIndexModel, cancellationToken: cancellationToken);

            var storyCollectionIndexKeys = Builders<StoryModel>.IndexKeys;
            var storyCollectionIndexModel = new CreateIndexModel<StoryModel>(
                storyCollectionIndexKeys.Ascending(i => i.SessionId).Ascending(i => i.SessionId)
            );
            await StoryModels.Indexes.CreateOneAsync(storyCollectionIndexModel, cancellationToken: cancellationToken);

            var userCollectionIndexKeys = Builders<UserModel>.IndexKeys;
            var userCollectionSessionIdIndexModel = new CreateIndexModel<UserModel>(
                userCollectionIndexKeys.Ascending(i => i.SessionId).Ascending(i => i.SessionId)
            );
            var userCollectionConnectionIdIndexModel = new CreateIndexModel<UserModel>(
                userCollectionIndexKeys.Ascending(i => i.ConnectionId).Ascending(i => i.ConnectionId)
            );

            var userCollectionIndexModels = new CreateIndexModel<UserModel>[] { userCollectionSessionIdIndexModel, userCollectionConnectionIdIndexModel };
            await UserModels.Indexes.CreateManyAsync(userCollectionIndexModels, cancellationToken: cancellationToken);
        }
    }
}