using System.Collections.Generic;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Models.Api
{
    public class SessionStateResponse
    {
        public string SessionId { get; set; }

        public string SessionCode { get; set; }

        public string UserId { get; set; }

        public bool IsHost { get; set; }

        public IEnumerable<StoryModel> Stories { get; set; }

        public IEnumerable<UserModel> Users { get; set; }

        public IEnumerable<string> PointChoices { get; set; }

        public bool HasStarted { get; set; }

        public bool HasFinished { get; set; }

        public string CurrentStoryId { get; set; }
    }
}