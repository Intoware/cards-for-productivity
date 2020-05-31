using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Repositories
{
    public interface ISessionRepo
    {
        Task InsertSessionAsync(SessionModel session, CancellationToken cancellationToken);

        Task<SessionModel> GetSessionByIdAsync(string sessionId, CancellationToken cancellationToken);

        Task<SessionModel> GetSessionBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken);

        Task<bool> DeleteSessionByIdAsync(string sessionId, CancellationToken cancellationToken);

        Task SetSessionStartedAsync(string sessionId, CancellationToken cancellationToken);

        Task SetCurrentStoryAsync(string sessionId, string storyId, CancellationToken cancellationToken);
    }
}