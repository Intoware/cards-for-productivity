using System;
using System.Collections.Generic;
using CardsForProductivity.API.Models.Api;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CardsForProductivity.API.Models.Data
{
    /// <summary>
    /// Session model.
    /// </summary>
    public class SessionModel
    {
        /// <summary>
        /// Session ID.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SessionId { get; set ;}

        /// <summary>
        /// Session code.
        /// </summary>
        public string SessionCode { get; set; }

        /// <summary>
        /// Date/time when the session expires.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Host actions authentication code.
        /// </summary>
        public string HostCode { get; set; }

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