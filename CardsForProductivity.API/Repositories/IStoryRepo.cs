using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Repositories
{
    public interface IStoryRepo
    {
        Task InsertStoriesAsync(IEnumerable<StoryModel> stories, CancellationToken cancellationToken);

        Task<IEnumerable<StoryModel>> GetStoriesBySessionIdAsync(string sessionId, CancellationToken cancellationToken);

        Task<StoryModel> GetStoryByIdAsync(string storyId, CancellationToken cancellationToken);

        Task<bool> DeleteStoriesBySessionIdAsync(string sessionId, CancellationToken cancellationToken);

        Task UpdateUserPointSelectionForStoryAsync(string storyId, string userId, string pointSelection, CancellationToken cancellationToken);

        Task RevealStoryAsync(string storyId, CancellationToken cancellationToken);
    }
}