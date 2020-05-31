using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Repositories
{
    public interface IUserRepo
    {
        Task InsertUserAsync(UserModel user, CancellationToken cancellationToken);

        Task UpdateRejoinCodeAsync(string userId, string newRejoinCode, CancellationToken cancellationToken);

        Task<UserModel> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

        Task<IEnumerable<UserModel>> GetUsersBySessionIdAsync(string sessionId, CancellationToken cancellationToken);

        Task<bool> DeleteUsersBySessionIdAsync(string sessionId, CancellationToken cancellationToken);

        Task SetConnectionIdAsync(string userId, string connectionId, CancellationToken cancellationToken);

        Task<UserModel> GetUserByConnectionIdAsync(string connectionId, CancellationToken cancellationToken);
    }
}