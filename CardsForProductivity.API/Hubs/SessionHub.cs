using System;
using System.Linq;
using System.Threading.Tasks;
using CardsForProductivity.API.Models.Api;
using CardsForProductivity.API.Models.Data;
using CardsForProductivity.API.Providers;
using CardsForProductivity.API.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace CardsForProductivity.API.Hubs
{
    /// <summary>
    /// Session hub.
    /// </summary>
    public class SessionHub : Hub
    {
        readonly ISessionProvider _sessionProvider;
        readonly IUserRepo _userRepo;
        readonly IStoryRepo _storyRepo;

        public SessionHub(ISessionProvider sessionProvider,
            IUserRepo userRepo,
            IStoryRepo storyRepo)
        {
            _sessionProvider = sessionProvider;
            _userRepo = userRepo;
            _storyRepo = storyRepo;
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            var user = await _userRepo.GetUserByConnectionIdAsync(Context.ConnectionId, default);
            await _userRepo.SetConnectionIdAsync(user.UserId, null, default);
            user.ConnectionId = null;

            await Clients.Group(user.SessionId).SendAsync("UserDisconnected", user);

            await base.OnDisconnectedAsync(e);
        }

        #region Host Methods
        /// <summary>
        /// Starts the session.
        /// </summary>
        /// <param name="sessionId">ID of the session.</param>
        /// <param name="hostCode">Authentication code for the host.</param>
        [HubMethodName("StartSession")]
        public async Task StartSessionAsync(string sessionId, string hostCode)
        {
            if (!await _sessionProvider.CheckSessionForHostAsync(sessionId, hostCode, default))
            {
                return;
            }

            await _sessionProvider.StartSessionAsync(sessionId, hostCode, default);

            await Clients.Group(sessionId).SendAsync("StartSession", sessionId);
        }

        /// <summary>
        /// Changes the current story.
        /// </summary>
        /// <param name="sessionId">ID of the session.</param>
        /// <param name="hostCode">Authentication code for the host.</param>
        /// <param name="storyID">ID of the story to change to.</param>
        [HubMethodName("CurrentStoryChanged")]
        public async Task SendCurrentStoryChangedMessageAsync(string sessionId, string hostCode, string storyId)
        {
            if (!await _sessionProvider.CheckSessionForHostAsync(sessionId, hostCode, default))
            {
                return;
            }

            if (storyId is null)
            {
                return;
            }

            await _sessionProvider.ChangeCurrentStoryAsync(sessionId, hostCode, storyId, default);

            await Clients.Group(sessionId).SendAsync("CurrentStoryChanged", storyId);
        }

        /// <summary>
        /// Reveals the current story.
        /// </summary>
        /// <param name="sessionId">ID of the session.</param>
        /// <param name="hostCode">Authentication code for the host.</param>
        [HubMethodName("RevealCurrentStory")]
        public async Task RevealCurrentStoryAsync(string sessionId, string hostCode)
        {
            if (!await _sessionProvider.CheckSessionForHostAsync(sessionId, hostCode, default))
            {
                return;
            }

            var session = await _sessionProvider.GetSessionByIdAsync(sessionId, default);
            var stories = await _storyRepo.GetStoriesBySessionIdAsync(sessionId, default);
            var currentStory = stories.FirstOrDefault(i => i.StoryId == session.CurrentStoryId);

            await _storyRepo.RevealStoryAsync(currentStory.StoryId, default);

            await Clients.Group(sessionId).SendAsync("RevealCurrentStory", sessionId);
        }

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <param name="sessionId">ID of the session.</param>
        /// <param name="hostCode">Authentication code for the host.</param>
        [HubMethodName("EndSession")]
        public async Task EndSessionAsync(string sessionId, string hostCode)
        {
            if (!await _sessionProvider.CheckSessionForHostAsync(sessionId, hostCode, default))
            {
                return;
            }

            await _sessionProvider.EndSessionAsync(sessionId, hostCode, default);

            await Clients.Group(sessionId).SendAsync("EndSession", sessionId);
        }

        /// <summary>
        /// Kicks a user from the session.
        /// </summary>
        /// <param name="sessionId">ID of the session.</param>
        /// <param name="hostCode">Authentication code for the host.</param>
        /// <param name="userId">ID of the user to kick.</param>
        [HubMethodName("KickUser")]
        public async Task KickUserAsync(string sessionId, string hostCode, string userId)
        {
            if (!await _sessionProvider.CheckSessionForHostAsync(sessionId, hostCode, default))
            {
                return;
            }

            var user = await _userRepo.GetUserByIdAsync(userId, default);

            if (user is null)
            {
                return;
            }

            var hostUser = await _userRepo.GetUserByConnectionIdAsync(Context.ConnectionId, default);

            if (hostUser.UserId == userId)
            {
                return;
            }

            await _userRepo.DeleteUserByIdAsync(userId, default);

            await Clients.Group(sessionId).SendAsync("UserKicked", user);
        }

        /// <summary>
        /// Updates the stories in a session.
        /// </summary>
        /// <param name="sessionId">ID of the session.</param>
        /// <param name="hostCode">Authentication code for the host.</param>
        /// <param name="stories">Stories.</param>
        [HubMethodName("UpdateStories")]
        public async Task UpdateStoriesInSessionAsync(string sessionId, string hostCode, StoryModel[] stories)
        {
            if (!await _sessionProvider.CheckSessionForHostAsync(sessionId, hostCode, default))
            {
                return;
            }

            if (stories is null || stories.Length == 0 || stories.Any(i => string.IsNullOrWhiteSpace(i.Title)))
            {
                return;
            }

            await _storyRepo.UpdatesStoriesAsync(sessionId, stories, default);

            await Clients.Group(sessionId).SendAsync("StoriesUpdated");
        }
        #endregion

        #region Client Methods
        /// <summary>
        /// Subscribes a client to the session's group.
        /// </summary>
        /// <param name="clientRequestDetails">Client request details.</param>
        [HubMethodName("Subscribe")]
        public async Task SubscribeAsync(ClientRequestDetails clientRequestDetails)
        {
            if (!await _sessionProvider.CheckSessionForClientAsync(clientRequestDetails, default))
            {
                return;
            }

            await AddToGroupAsync(clientRequestDetails.SessionId);

            await _userRepo.SetConnectionIdAsync(clientRequestDetails.UserId, Context.ConnectionId, default);

            var user = await _userRepo.GetUserByIdAsync(clientRequestDetails.UserId, default);

            await Clients.GroupExcept(clientRequestDetails.SessionId, Context.ConnectionId).SendAsync("UserConnected", user);

            var users = await _userRepo.GetUsersBySessionIdAsync(clientRequestDetails.SessionId, default);
            await Clients.Caller.SendAsync("UserList", users);
        }

        /// <summary>
        /// Gets the current state of the session.
        /// </summary>
        /// <param name="clientRequestDetails">Client request details.</param>
        [HubMethodName("GetSessionState")]
        public async Task GetSessionStateAsync(ClientRequestDetails clientRequestDetails)
        {
            if (!await _sessionProvider.CheckSessionForClientAsync(clientRequestDetails, default))
            {
                return;
            }

            var session = await _sessionProvider.GetSessionByIdAsync(clientRequestDetails.SessionId, default);
            var users = await _userRepo.GetUsersBySessionIdAsync(clientRequestDetails.SessionId, default);
            var stories = await _storyRepo.GetStoriesBySessionIdAsync(clientRequestDetails.SessionId, default);

            var sessionState = new SessionStateResponse
            {
                SessionId = session.SessionId,
                SessionCode = session.SessionCode,
                UserId = clientRequestDetails.UserId,
                IsHost = users.FirstOrDefault(i => i.UserId == clientRequestDetails.UserId)?.IsHost ?? false,
                Stories = stories.OrderBy(i => i.StoryIndex),
                Users = users,
                PointChoices = session.PointChoices,
                HasStarted = session.HasStarted,
                HasFinished = session.HasFinished,
                CurrentStoryId = session.CurrentStoryId
            };

            await Clients.Caller.SendAsync("GetSessionState", sessionState);
        }

        /// <summary>
        /// Makes a point selection for the user for the current round.
        /// </summary>
        /// <param name="clientRequestDetails">Client request details.</param>
        /// <param name="points">Points to assign to the current story.</param>
        [HubMethodName("MakePointSelection")]
        public async Task MakePointSelectionForCurrentStoryAsync(ClientRequestDetails clientRequestDetails, string points)
        {
            if (!await _sessionProvider.CheckSessionForClientAsync(clientRequestDetails, default))
            {
                return;
            }

            var session = await _sessionProvider.GetSessionByIdAsync(clientRequestDetails.SessionId, default);

            await _storyRepo.UpdateUserPointSelectionForStoryAsync(
                session.CurrentStoryId,
                clientRequestDetails.UserId,
                points,
                default);

            var story = await _storyRepo.GetStoryByIdAsync(session.CurrentStoryId, default);

            await Clients.Group(session.SessionId).SendAsync("MakePointSelection", story);
        }
        #endregion

        Task AddToGroupAsync(string sessionId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }

        Task RemoveFromGroupAsync(string sessionId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        }
    }
}