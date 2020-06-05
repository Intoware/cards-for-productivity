using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Repositories
{
    /// <summary>
    /// Session repository.
    /// </summary>
    public interface ISessionRepo
    {
        /// <summary>
        /// Inserts a session.
        /// </summary>
        /// <param name="session">Session to insert.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task InsertSessionAsync(SessionModel session, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a session by its ID.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Session model.</returns>
        Task<SessionModel> GetSessionByIdAsync(string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a session by its code.
        /// </summary>
        /// <param name="sessionCode">Session code.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Session model.</returns>
        Task<SessionModel> GetSessionBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a session by its ID.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if successful, else false.</returns>
        Task<bool> DeleteSessionByIdAsync(string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the session to started.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SetSessionStartedAsync(string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the session to finished.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SetSessionFinishedAsync(string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the current story in the session.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="storyId">storyId ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SetCurrentStoryAsync(string sessionId, string storyId, CancellationToken cancellationToken);
    }
}