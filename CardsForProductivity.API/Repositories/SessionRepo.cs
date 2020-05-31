using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;
using CardsForProductivity.API.Providers;
using MongoDB.Driver;

namespace CardsForProductivity.API.Repositories
{
    public class SessionRepo : ISessionRepo
    {
        readonly IMongoCollection<SessionModel> _sessionCollection;

        public SessionRepo(IDbContext dbContext)
        {
            _sessionCollection = dbContext.SessionModels;
        }

        public Task InsertSessionAsync(SessionModel session, CancellationToken cancellationToken)
        {
            _ = session ?? throw new ArgumentNullException(nameof(session));

            return _sessionCollection.InsertOneAsync(session, cancellationToken: cancellationToken);
        }

        public async Task<SessionModel> GetSessionByIdAsync(string sessionId, CancellationToken cancellationToken)
        {
            _ = sessionId ?? throw new ArgumentNullException(nameof(sessionId));

            var filter = Builders<SessionModel>.Filter.Eq(i => i.SessionId, sessionId);

            return (await _sessionCollection.FindAsync(filter, cancellationToken: cancellationToken)).FirstOrDefault();
        }

        public async Task<SessionModel> GetSessionBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken)
        {
            _ = sessionCode ?? throw new ArgumentNullException(nameof(sessionCode));

            var filter = Builders<SessionModel>.Filter.Eq(i => i.SessionCode, sessionCode);

            return (await _sessionCollection.FindAsync(filter, cancellationToken: cancellationToken)).FirstOrDefault();
        }

        public async Task<bool> DeleteSessionByIdAsync(string sessionId, CancellationToken cancellationToken)
        {
            _ = sessionId ?? throw new ArgumentNullException(nameof(sessionId));

            var filter = Builders<SessionModel>.Filter.Eq(i => i.SessionId, sessionId);

            return (await _sessionCollection.DeleteOneAsync(filter, cancellationToken)).DeletedCount == 1;
        }

        public Task SetSessionStartedAsync(string sessionId, CancellationToken cancellationToken)
        {
            _ = sessionId ?? throw new ArgumentNullException(nameof(sessionId));

            var filter = Builders<SessionModel>.Filter.Eq(i => i.SessionId, sessionId);
            var update = Builders<SessionModel>.Update.Set(i => i.HasStarted, true);

            return _sessionCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public Task SetCurrentStoryAsync(string sessionId, string storyId, CancellationToken cancellationToken)
        {
            _ = sessionId ?? throw new ArgumentNullException(nameof(sessionId));

            var filter = Builders<SessionModel>.Filter.Eq(i => i.SessionId, sessionId);
            var update = Builders<SessionModel>.Update.Set(i => i.CurrentStoryId, storyId);

            return _sessionCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}