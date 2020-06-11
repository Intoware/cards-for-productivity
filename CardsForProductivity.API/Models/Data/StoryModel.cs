using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CardsForProductivity.API.Models.Data
{
    public class StoryModel
    {
        /// <summary>
        /// Story ID.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StoryId { get; set; }

        /// <summary>
        /// Session ID.
        /// </summary>
        public string SessionId { get; set; }
        
        /// <summary>
        /// Title of the story.
        /// </summary>
        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        /// <summary>
        /// Description of the story.
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Acceptance criteria of the story.
        /// </summary>
        public string AcceptanceCriteria { get; set; }

        /// <summary>
        /// Dictionary containing which points each user has selected.
        /// </summary>
        /// <typeparam name="string">User ID.</typeparam>
        /// <typeparam name="string">Point selection for that user.</typeparam>
        public IDictionary<string, string> UserPoints { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Whether the story has been revealed.
        /// </summary>
        public bool Revealed { get; set; }

        /// <summary>
        /// Index of the story.
        /// </summary>
        public int StoryIndex { get; set; }
    }
}