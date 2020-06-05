using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CardsForProductivity.API.Models.Api
{
    /// <summary>
    /// Create session response.
    /// </summary>
    public class CreateSessionResponse
    {
        /// <summary>
        /// Session ID.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SessionId { get; set; }

        /// <summary>
        /// Session code.
        /// </summary>
        public string SessionCode { get; set; }

        /// <summary>
        /// Host actions authentication code.
        /// </summary>
        public string HostCode { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Client actions authentication code.
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// Point that users can select in the session.
        /// </summary>
        public IEnumerable<string> PointChoices { get; set; }
    }
}