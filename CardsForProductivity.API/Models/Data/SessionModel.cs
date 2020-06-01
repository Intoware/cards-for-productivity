using System;
using System.Collections.Generic;
using CardsForProductivity.API.Models.Api;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CardsForProductivity.API.Models.Data
{
    public class SessionModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SessionId { get; set ;}

        public string SessionCode { get; set; }

        public DateTime Expires { get; set; }

        public string HostCode { get; set; }

        public IEnumerable<string> PointChoices { get; set; }

        public bool HasStarted { get; set; }

        public bool HasFinished { get; set; }

        public string CurrentStoryId { get; set; }
    }
}