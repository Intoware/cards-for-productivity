using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CardsForProductivity.API.Models.Api
{
    public class CreateSessionResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SessionId { get; set; }

        public string SessionCode { get; set; }

        public string HostCode { get; set; }

        public string UserId { get; set; }

        public string AuthCode { get; set; }

        public IEnumerable<string> PointChoices { get; set; }
    }
}