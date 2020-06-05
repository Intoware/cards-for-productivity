using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;
using CardsForProductivity.API.Providers;
using MongoDB.Driver;

namespace CardsForProductivity.API.Repositories
{
    public class UserRepo : IUserRepo
    {
        readonly IMongoCollection<UserModel> _userCollection;

        public UserRepo(IDbContext dbContext)
        {
            _userCollection = dbContext.UserModels;
        }

        public Task InsertUserAsync(UserModel user, CancellationToken cancellationToken)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            return _userCollection.InsertOneAsync(user, cancellationToken: cancellationToken);
        }

        public Task UpdateRejoinCodeAsync(string userId, string newRejoinCode, CancellationToken cancellationToken)
        {
            _ = userId ?? throw new ArgumentNullException(nameof(userId));
            _ = newRejoinCode ?? throw new ArgumentNullException(nameof(newRejoinCode));

            var filter = Builders<UserModel>.Filter.Eq(i => i.UserId, userId);
            var update = Builders<UserModel>.Update.Set(i => i.RejoinCode, newRejoinCode);

            return _userCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task<UserModel> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            _ = userId ?? throw new ArgumentNullException(nameof(userId));

            var filter = Builders<UserModel>.Filter.Eq(i => i.UserId, userId);

            return (await _userCollection.FindAsync(filter)).FirstOrDefault();
        }

        public async Task<IEnumerable<UserModel>> GetUsersBySessionIdAsync(string sessionId, CancellationToken cancellationToken)
        {
            _ = sessionId ?? throw new ArgumentNullException(nameof(sessionId));

            var filter = Builders<UserModel>.Filter.Eq(i => i.SessionId, sessionId);
            var projection = Builders<UserModel>.Projection
                .Exclude(i => i.RejoinCode)
                .Exclude(i => i.AuthCode);

            return await _userCollection.Find(filter).Project<UserModel>(projection).ToListAsync(cancellationToken);
        }

        public async Task<bool> DeleteUsersBySessionIdAsync(string sessionId, CancellationToken cancellationToken)
        {
            _ = sessionId ?? throw new ArgumentNullException(nameof(sessionId));

            var filter = Builders<UserModel>.Filter.Eq(i => i.SessionId, sessionId);

            return (await _userCollection.DeleteManyAsync(filter, cancellationToken)).DeletedCount > 1;
        }

        public async Task<bool> DeleteUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            _ = userId ?? throw new ArgumentNullException(nameof(userId));

            var filter = Builders<UserModel>.Filter.Eq(i => i.UserId, userId);

            return (await _userCollection.DeleteOneAsync(filter, cancellationToken)).DeletedCount == 1;
        }

        public Task SetConnectionIdAsync(string userId, string connectionId, CancellationToken cancellationToken)
        {
            _ = userId ?? throw new ArgumentNullException(nameof(userId));

            var filter = Builders<UserModel>.Filter.Eq(i => i.UserId, userId);
            var update = Builders<UserModel>.Update.Set(i => i.ConnectionId, connectionId);
        
            return _userCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task<UserModel> GetUserByConnectionIdAsync(string connectionId, CancellationToken cancellationToken)
        {
            _ = connectionId ?? throw new ArgumentNullException(nameof(connectionId));

            var filter = Builders<UserModel>.Filter.Eq(i => i.ConnectionId, connectionId);

            return (await _userCollection.FindAsync(filter)).FirstOrDefault();
        }
    }
}