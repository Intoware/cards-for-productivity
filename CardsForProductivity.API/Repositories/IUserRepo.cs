using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Repositories
{
    /// <summary>
    /// User repository.
    /// </summary>
    public interface IUserRepo
    {
        /// <summary>
        /// Inserts a user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task InsertUserAsync(UserModel user, CancellationToken cancellationToken);

        /// <summary>
        /// Update the rejoin code for the given user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="newRejoinCode">New rejoin code.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task UpdateRejoinCodeAsync(string userId, string newRejoinCode, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a user by its ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User model.</returns>
        Task<UserModel> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an enumerable of users in a session.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Enumerable of UserModels.</returns>
        Task<IEnumerable<UserModel>> GetUsersBySessionIdAsync(string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes all of the users in a session.
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if one or more users were deleted, else false.</returns>
        Task<bool> DeleteUsersBySessionIdAsync(string sessionId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a user by its ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if successful, else false.</returns>
        Task<bool> DeleteUserByIdAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the connection ID of the given user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="connectionId">Connection ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SetConnectionIdAsync(string userId, string connectionId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a user by its connection ID.
        /// </summary>
        /// <param name="connectionId">Connection ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User model.</returns>
        Task<UserModel> GetUserByConnectionIdAsync(string connectionId, CancellationToken cancellationToken);
    }
}