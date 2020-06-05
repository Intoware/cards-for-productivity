using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CardsForProductivity.API.Models.Data
{
    public class UserModel
    {
        /// <summary>
        /// User ID.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        /// <summary>
        /// Session ID.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Rejoin code.
        /// </summary>
        public string RejoinCode { get; set; }

        /// <summary>
        /// Client actions authentication code.
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// Whether the user is the host.
        /// </summary>
        public bool IsHost { get; set; }

        /// <summary>
        /// SignalR connection ID.
        /// </summary>
        [JsonIgnore]
        public string ConnectionId { get; set; }

        /// <summary>
        /// Whether the user is currently connected to the SessionHub.
        /// </summary>
        [BsonIgnore]
        public bool IsOnline => !string.IsNullOrEmpty(ConnectionId);
    }
}