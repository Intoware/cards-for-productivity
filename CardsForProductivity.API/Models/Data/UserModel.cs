using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CardsForProductivity.API.Models.Data
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public string SessionId { get; set; }

        public string Nickname { get; set; }

        public string RejoinCode { get; set; }

        public string AuthCode { get; set; }

        public bool IsHost { get; set; }

        [JsonIgnore]
        public string ConnectionId { get; set; }

        [BsonIgnore]
        public bool IsOnline => !string.IsNullOrEmpty(ConnectionId);
    }
}