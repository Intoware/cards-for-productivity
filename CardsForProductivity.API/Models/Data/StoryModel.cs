using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CardsForProductivity.API.Models.Data
{
    public class StoryModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StoryId { get; set; }

        public string SessionId { get; set; }
        
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string AcceptanceCriteria { get; set; }

        public string Points { get; set; }

        public IDictionary<string, string> UserPoints { get; set; } = new Dictionary<string, string>();

        public bool Revealed { get; set; }
    }
}