using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Api;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Providers
{
    /// <summary>
    /// Session provider.
    /// </summary>
    public interface ISessionProvider
    {
        /// <summary>
        /// Creates a session.
        /// </summary>
        /// <param name="createSessionRequest">Create session request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Create session response.</returns>
        Task<CreateSessionResponse> CreateSessionAsync(CreateSessionRequest createSessionRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a session by its ID.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Session model.</returns>
        Task<SessionModel> GetSessionByIdAsync(string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a session by its session code.
        /// </summary>
        /// <param name="sessionCode">Session code.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Session model.</returns>
        Task<SessionModel> GetSessionBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken);

        /// <summary>
        /// Joins a session.
        /// </summary>
        /// <param name="joinSessionRequest">Join session request.</param>
        /// <param name="sessionId">Session ID to join.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Join session response.</returns>
        Task<JoinSessionResponse> JoinSessionAsync(JoinSessionRequest joinSessionRequest, string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether a user exists in the session by their nickname.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="nickname">Nickname.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if a user with the given nickname exists in the session, else false.</returns>
        Task<bool> DoesUserExistInSessionAsync(string sessionId, string nickname, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User model.</returns>
        Task<UserModel> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether a client can perform an action in a session.
        /// </summary>
        /// <param name="clientRequestDetails">Client request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the client can perform an action, else false.</returns>
        Task<bool> CheckSessionForClientAsync(ClientRequestDetails clientRequestDetails, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether a host can perform an action in a session.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="hostCode">Host code.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the host can perform an action, else false.</returns>
        Task<bool> CheckSessionForHostAsync(string sessionId, string hostCode, CancellationToken cancellationToken);

        /// <summary>
        /// Starts a session.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="hostCode">Host code.</param>
        /// <param name="cancellationToken">Cancellation token.</param>    
        Task StartSessionAsync(string sessionId, string hostCode, CancellationToken cancellationToken);

        /// <summary>
        /// Ends a session.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="hostCode">Host code.</param>
        /// <param name="cancellationToken">Cancellation token.</param>    
        Task EndSessionAsync(string sessionId, string hostCode, CancellationToken cancellationToken);

        /// <summary>
        /// Changes the current story for a session.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="hostCode">Host code.</param>
        /// <param name="storyId">Story ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task ChangeCurrentStoryAsync(string sessionId, string hostCode, string storyId, CancellationToken cancellationToken);
    }
}