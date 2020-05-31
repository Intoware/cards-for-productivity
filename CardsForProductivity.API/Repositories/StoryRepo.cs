using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;
using CardsForProductivity.API.Providers;
using MongoDB.Driver;

namespace CardsForProductivity.API.Repositories
{
    public class StoryRepo : IStoryRepo
    {
        readonly IMongoCollection<StoryModel> _storyCollection;

        public StoryRepo(IDbContext dbContext)
        {
            _storyCollection = dbContext.StoryModels;
        }

        public Task InsertStoriesAsync(IEnumerable<StoryModel> stories, CancellationToken cancellationToken)
        {
            _ = stories ?? throw new ArgumentNullException(nameof(stories));

            return _storyCollection.InsertManyAsync(stories, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<StoryModel>> GetStoriesBySessionIdAsync(string sessionId, CancellationToken cancellationToken)
        {
            _ = sessionId ?? throw new ArgumentNullException(nameof(sessionId));

            var filter = Builders<StoryModel>.Filter.Eq(i => i.SessionId, sessionId);

            return await _storyCollection.Find(filter).ToListAsync(cancellationToken);
        }

        public Task<StoryModel> GetStoryByIdAsync(string storyId, CancellationToken cancellationToken)
        {
            _ = storyId ?? throw new ArgumentNullException(nameof(storyId));

            var filter = Builders<StoryModel>.Filter.Eq(i => i.StoryId, storyId);

            return _storyCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> DeleteStoriesBySessionIdAsync(string sessionId, CancellationToken cancellationToken)
        {
            _ = sessionId ?? throw new ArgumentNullException(nameof(sessionId));

            var filter = Builders<StoryModel>.Filter.Eq(i => i.SessionId, sessionId);

            return (await _storyCollection.DeleteManyAsync(filter, cancellationToken)).DeletedCount > 1;
        }

        public async Task UpdateUserPointSelectionForStoryAsync(string storyId, string userId, string pointSelection, CancellationToken cancellationToken)
        {
            _ = storyId ?? throw new ArgumentNullException(nameof(storyId));
            _ = userId ?? throw new ArgumentNullException(nameof(userId));
            _ = pointSelection ?? throw new ArgumentNullException(nameof(pointSelection));

            var filter = Builders<StoryModel>.Filter.Eq(i => i.StoryId, storyId);

            var currentStory = await _storyCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            currentStory.UserPoints[userId] = pointSelection;

            var update = Builders<StoryModel>.Update.Set(i => i.UserPoints, currentStory.UserPoints);

            await _storyCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public Task RevealStoryAsync(string storyId, CancellationToken cancellationToken)
        {
            _ = storyId ?? throw new ArgumentNullException(nameof(storyId));

            var filter = Builders<StoryModel>.Filter.Eq(i => i.StoryId, storyId);
            var update = Builders<StoryModel>.Update.Set(i => i.Revealed, true);

            return _storyCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}