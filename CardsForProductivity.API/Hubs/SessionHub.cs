using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using CardsForProductivity.API.Helpers;
using CardsForProductivity.API.Models.Api;
using CardsForProductivity.API.Providers;
using CardsForProductivity.API.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace CardsForProductivity.API.Hubs
{
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

        [HubMethodName("CurrentStoryChanged")]
        public async Task SendCurrentStoryChangedMessageAsync(string sessionId, string hostCode, string storyId)
        {
            if (!await _sessionProvider.CheckSessionForHostAsync(sessionId, hostCode, default))
            {
                return;
            }

            await _sessionProvider.ChangeCurrentStoryAsync(sessionId, hostCode, storyId, default);

            await Clients.Group(sessionId).SendAsync("CurrentStoryChanged", storyId);
        }

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

        [HubMethodName("EndSession")]
        public async Task EndSessionAsync(string sessionId, string hostCode)
        {
            if (!await _sessionProvider.CheckSessionForHostAsync(sessionId, hostCode, default))
            {
                return;
            }

            await Clients.Group(sessionId).SendAsync("EndSession", sessionId);
        }

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
                Stories = stories,
                Users = users,
                PointChoices = session.PointChoices,
                HasStarted = session.HasStarted,
                HasFinished = session.HasFinished,
                CurrentStoryId = session.CurrentStoryId
            };

            await Clients.Caller.SendAsync("GetSessionState", sessionState);
        }

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