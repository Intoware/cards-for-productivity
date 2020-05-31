using System.Threading;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Data;
using MongoDB.Driver;

namespace CardsForProductivity.API.Repositories
{
    public interface IDbContext
    {
        IMongoCollection<SessionModel> SessionModels { get; }

        IMongoCollection<StoryModel> StoryModels { get; }

        IMongoCollection<UserModel> UserModels { get; }

        Task SetupAsync(CancellationToken token);
    }
}