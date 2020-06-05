using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Repositories
{
    /// <summary>
    /// Story repository.
    /// </summary>
    public interface IStoryRepo
    {
        /// <summary>
        /// Inserts an enumerable of stories.
        /// </summary>
        /// <param name="stories">Stories.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task InsertStoriesAsync(IEnumerable<StoryModel> stories, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an enumerable of stories in a session.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Enumerable of StoryModels.</returns>
        Task<IEnumerable<StoryModel>> GetStoriesBySessionIdAsync(string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a story by its ID.
        /// </summary>
        /// <param name="storyId">Story ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Story model.</returns>
        Task<StoryModel> GetStoryByIdAsync(string storyId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes all of the stories in a session.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if one or more stories were deleted, else false.</returns>
        Task<bool> DeleteStoriesBySessionIdAsync(string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a user's points selection for a story.
        /// </summary>
        /// <param name="storyId">Story ID.</param>
        /// <param name="userId">User ID.</param>
        /// <param name="pointSelection">Point selection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task UpdateUserPointSelectionForStoryAsync(string storyId, string userId, string pointSelection, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the story to revealed.
        /// </summary>
        /// <param name="storyId">Story ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RevealStoryAsync(string storyId, CancellationToken cancellationToken);
    }
}