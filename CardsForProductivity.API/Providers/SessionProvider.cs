using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Api;
using CardsForProductivity.API.Repositories;
using MongoDB.Bson;
using Hangfire;
using CardsForProductivity.API.Models.Data;
using CardsForProductivity.API.Helpers;

namespace CardsForProductivity.API.Providers
{
    public class SessionProvider : ISessionProvider
    {
        static Random random = new Random();

        const int SessionCodeLength = 4;
        const int HostCodeLength = 30;
        const int AuthCodeLength = 30;
        const int ExpiryTimeInDays = 1;

        readonly ISessionRepo _sessionRepo;
        readonly IStoryRepo _storyRepo;
        readonly IUserRepo _userRepo;
        readonly IBackgroundJobClient _backgroundJobClient;

        public SessionProvider(ISessionRepo sessionRepo,
            IStoryRepo storyRepo,
            IUserRepo userRepo,
            IBackgroundJobClient backgroundJobClient)
        {
            _sessionRepo = sessionRepo;
            _storyRepo = storyRepo;
            _userRepo = userRepo;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<CreateSessionResponse> CreateSessionAsync(CreateSessionRequest createSessionRequest, CancellationToken cancellationToken)
        {
            var session = new SessionModel
            {
                SessionId = ObjectId.GenerateNewId().ToString(),
                SessionCode = GetRandomCode(SessionCodeLength),
                Expires = DateTime.UtcNow.AddDays(ExpiryTimeInDays),
                HostCode = GetRandomCode(HostCodeLength),
                PointChoices = createSessionRequest.PointChoices
            };

            await _sessionRepo.InsertSessionAsync(session, cancellationToken);

            var user = new UserModel
            {
                SessionId = session.SessionId,
                UserId = ObjectId.GenerateNewId().ToString(),
                AuthCode = GetRandomCode(AuthCodeLength),
                Nickname = createSessionRequest.Nickname,
                IsHost = true
            };

            await _userRepo.InsertUserAsync(user, cancellationToken);

            var i = 0;
            foreach (var story in createSessionRequest.Stories)
            {
                story.SessionId = session.SessionId;
                story.StoryIndex = i++;
            }

            await _storyRepo.InsertStoriesAsync(createSessionRequest.Stories, cancellationToken);

            ScheduleDeleteSession(session.SessionId);

            return new CreateSessionResponse 
            { 
                SessionId = session.SessionId,
                SessionCode = session.SessionCode,
                HostCode = session.HostCode,
                PointChoices = session.PointChoices,
                UserId = user.UserId,
                AuthCode = user.AuthCode
            };
        }

        public Task<SessionModel> GetSessionByIdAsync(string sessionId, CancellationToken cancellationToken)
        {
            return _sessionRepo.GetSessionByIdAsync(sessionId, cancellationToken);
        }

        public Task<SessionModel> GetSessionBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken)
        {
            return _sessionRepo.GetSessionBySessionCodeAsync(sessionCode.ToUpperInvariant(), cancellationToken);
        }

        public async Task<JoinSessionResponse> JoinSessionAsync(JoinSessionRequest joinSessionRequest, string sessionId, CancellationToken cancellationToken)
        {
            var rejoinCode = GetRandomCode(SessionCodeLength);
            var authCode = GetRandomCode(AuthCodeLength);
            var userId = joinSessionRequest.UserId ?? ObjectId.GenerateNewId().ToString();

            var session = await _sessionRepo.GetSessionByIdAsync(sessionId, cancellationToken);

            if (joinSessionRequest.UserId is null)
            {
                var user = new UserModel
                {
                    SessionId = session.SessionId,
                    UserId = userId,
                    Nickname = joinSessionRequest.Nickname,
                    RejoinCode = rejoinCode,
                    AuthCode = authCode
                };

                await _userRepo.InsertUserAsync(user, cancellationToken);
            }
            else
            {
                await _userRepo.UpdateRejoinCodeAsync(userId, rejoinCode, cancellationToken);
            }

            var stories = await _storyRepo.GetStoriesBySessionIdAsync(session.SessionId, cancellationToken);
            var users = await _userRepo.GetUsersBySessionIdAsync(session.SessionId, cancellationToken);

            return new JoinSessionResponse
            {
                SessionId = session.SessionId,
                UserId = userId,
                RejoinCode = rejoinCode,
                AuthCode = authCode,
                Stories = stories.OrderBy(i => i.StoryIndex),
                Users = users,
                PointChoices = session.PointChoices,
                HasStarted = session.HasStarted,
                HasFinished = session.HasFinished
            };
        }

        public async Task<bool> DoesUserExistInSessionAsync(string sessionId, string nickname, CancellationToken cancellationToken)
        {
            var users = await _userRepo.GetUsersBySessionIdAsync(sessionId, cancellationToken);

            return users.Any(i => i.Nickname == nickname);
        }

        public Task<UserModel> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (userId is null)
            {
                return null;
            }

            return _userRepo.GetUserByIdAsync(userId, cancellationToken);
        }

        public async Task<bool> CheckSessionForClientAsync(ClientRequestDetails clientRequestDetails, CancellationToken cancellationToken)
        {
            if (!ValidationHelper.ValidateObjectId(clientRequestDetails.SessionId))
            {
                return false;
            }

            var session = await GetSessionByIdAsync(clientRequestDetails.SessionId, cancellationToken);

            if (session is null)
            {
                return false;
            }

            if (session.SessionCode != clientRequestDetails.SessionCode.ToUpperInvariant())
            {
                return false;
            }

            var user = await _userRepo.GetUserByIdAsync(clientRequestDetails.UserId, cancellationToken);

            if (user is null)
            {
                return false;
            }

            if (user.AuthCode != clientRequestDetails.AuthCode)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CheckSessionForHostAsync(string sessionId, string hostCode, CancellationToken cancellationToken)
        {
            if (!ValidationHelper.ValidateObjectId(sessionId))
            {
                return false;
            }

            var session = await GetSessionByIdAsync(sessionId, cancellationToken);

            if (session is null)
            {
                return false;
            }

            if (session.HostCode != hostCode.ToUpperInvariant())
            {
                return false;
            }

            return true;
        }

        public async Task StartSessionAsync(string sessionId, string hostCode, CancellationToken cancellationToken)
        {
            if (!await CheckSessionForHostAsync(sessionId, hostCode, cancellationToken))
            {
                return;
            }

            var stories = await _storyRepo.GetStoriesBySessionIdAsync(sessionId, cancellationToken);
            await _sessionRepo.SetCurrentStoryAsync(sessionId, stories.First().StoryId, cancellationToken);

            await _sessionRepo.SetSessionStartedAsync(sessionId, cancellationToken);
        }

        public async Task EndSessionAsync(string sessionId, string hostCode, CancellationToken cancellationToken)
        {
            if (!await CheckSessionForHostAsync(sessionId, hostCode, cancellationToken))
            {
                return;
            }

            await _sessionRepo.SetSessionFinishedAsync(sessionId, cancellationToken);
        }

        public async Task ChangeCurrentStoryAsync(string sessionId, string hostCode, string storyId, CancellationToken cancellationToken)
        {
            if (!await CheckSessionForHostAsync(sessionId, hostCode, cancellationToken))
            {
                return;
            }

            await _sessionRepo.SetCurrentStoryAsync(sessionId, storyId, cancellationToken);
        }

        void ScheduleDeleteSession(string sessionId)
        {
            _backgroundJobClient.Schedule<ISessionRepo>(i => i.DeleteSessionByIdAsync(sessionId, CancellationToken.None), TimeSpan.FromDays(ExpiryTimeInDays));
            _backgroundJobClient.Schedule<IStoryRepo>(i => i.DeleteStoriesBySessionIdAsync(sessionId, CancellationToken.None), TimeSpan.FromDays(ExpiryTimeInDays));
            _backgroundJobClient.Schedule<IUserRepo>(i => i.DeleteUsersBySessionIdAsync(sessionId, CancellationToken.None), TimeSpan.FromDays(ExpiryTimeInDays));
        }

        static string GetRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}