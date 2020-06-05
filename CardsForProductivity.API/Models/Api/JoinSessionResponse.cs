using System.Collections.Generic;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Models.Api
{
    /// <summary>
    /// Join session response.
    /// </summary>
    public class JoinSessionResponse
    {
        /// <summary>
        /// Session ID.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Rejoin code.
        /// </summary>
        public string RejoinCode { get; set; }

        /// <summary>
        /// Client actions authentication code.
        /// </summary>
        public string AuthCode { get; set; }

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
    }
}