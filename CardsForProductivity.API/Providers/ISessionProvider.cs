using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Api;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Providers
{
    public interface ISessionProvider
    {
        Task<CreateSessionResponse> CreateSessionAsync(CreateSessionRequest createSessionRequest, CancellationToken cancellationToken);

        Task<SessionModel> GetSessionByIdAsync(string sessionId, CancellationToken cancellationToken);

        Task<SessionModel> GetSessionBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken);

        Task<JoinSessionResponse> JoinSessionAsync(JoinSessionRequest joinSessionRequest, string sessionId, CancellationToken cancellationToken);

        Task<bool> DoesUserExistInSessionAsync(string sessionId, string nickname, CancellationToken cancellationToken);

        Task<UserModel> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

        Task<bool> CheckSessionForClientAsync(ClientRequestDetails clientRequestDetails, CancellationToken cancellationToken);

        Task<bool> CheckSessionForHostAsync(string sessionId, string hostCode, CancellationToken cancellationToken);
        
        Task StartSessionAsync(string sessionId, string hostCode, CancellationToken cancellationToken);

        Task ChangeCurrentStoryAsync(string sessionId, string hostCode, string storyId, CancellationToken cancellationToken);
    }
}