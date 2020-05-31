using System.Collections.Generic;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Models.Api
{
    public class JoinSessionResponse
    {
        public string SessionId { get; set; }

        public string UserId { get; set; }

        public string RejoinCode { get; set; }

        public string AuthCode { get; set; }

        public IEnumerable<StoryModel> Stories { get; set; }

        public IEnumerable<UserModel> Users { get; set; }

        public IEnumerable<string> PointChoices { get; set; }

        public bool HasStarted { get; set; }
    }
}