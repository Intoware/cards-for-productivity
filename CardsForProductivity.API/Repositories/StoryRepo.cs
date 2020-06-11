using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;
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

        public async Task UpdatesStoriesAsync(string sessionId, IEnumerable<StoryModel> stories, CancellationToken cancellationToken)
        {
            _ = sessionId ?? throw new ArgumentNullException(nameof(sessionId));

            if (stories is null || !stories.Any())
            {
                throw new ArgumentNullException(nameof(stories));
            }

            var i = 0;
            foreach (var story in stories)
            {
                story.StoryIndex = i++;
            }

            var existingStories = await GetStoriesBySessionIdAsync(sessionId, cancellationToken);

            var newStories = stories.Where(i => !existingStories.Any(j => j.StoryId == i.StoryId));

            if (newStories.Any())
            {
                foreach (var story in newStories)
                {
                    story.SessionId = sessionId;
                }
                await _storyCollection.InsertManyAsync(newStories, cancellationToken: cancellationToken);
            }

            var filterBuilder = Builders<StoryModel>.Filter;

            var storiesToUpdate = existingStories.Where(i => stories.Any(j => j.StoryId == i.StoryId));

            foreach (var story in storiesToUpdate)
            {
                var filter = filterBuilder.And(
                    filterBuilder.Eq(i => i.SessionId, sessionId),
                    filterBuilder.Eq(i => i.StoryId, story.StoryId)
                );

                var updatedStory = stories.FirstOrDefault(i => i.StoryId == story.StoryId);

                var update = Builders<StoryModel>.Update
                    .Set(i => i.Title, updatedStory.Title)
                    .Set(i => i.Description, updatedStory.Description)
                    .Set(i => i.StoryIndex, updatedStory.StoryIndex);

                await _storyCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            }
            
            var deletedStories = existingStories.Where(i => !stories.Any(j => j.StoryId == i.StoryId));

            foreach (var story in deletedStories)
            {
                var filter = filterBuilder.And(
                    filterBuilder.Eq(i => i.SessionId, sessionId),
                    filterBuilder.Eq(i => i.StoryId, story.StoryId)
                );

                await _storyCollection.DeleteOneAsync(filter, cancellationToken: cancellationToken);
            }
        }
    }
}