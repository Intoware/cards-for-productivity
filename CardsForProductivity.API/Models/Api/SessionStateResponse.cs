using System.Collections.Generic;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Models.Api
{
    /// <summary>
    /// Session state response.
    /// </summary>
    public class SessionStateResponse
    {
        /// <summary>
        /// Session ID.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Session code.
        /// </summary>
        public string SessionCode { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Whether the user is the host.
        /// </summary>
        public bool IsHost { get; set; }

        /// <summary>
        /// Stories in the session.
        /// </summary>
        public IEnumerable<StoryModel> Stories { get; set; }

        /// <summary>
        /// Users in the session.
        /// </summary>
        public IEnumerable<UserModel> Users { get; set; }

        /// <summary>
        /// Point that users can select in the session.
        /// </summary>
        public IEnumerable<string> PointChoices { get; set; }

        /// <summary>
        /// Whether the session has started.
        /// </summary>
        public bool HasStarted { get; set; }

        /// <summary>
        /// Whether the session has finished.
        /// </summary>
        public bool HasFinished { get; set; }

        /// <summary>
        /// ID of the current story.
        /// </summary>
        public string CurrentStoryId { get; set; }
    }
}