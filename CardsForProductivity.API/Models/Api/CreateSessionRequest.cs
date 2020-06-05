using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Models.Api
{
    public class CreateSessionRequest
    {
        /// <summary>
        /// Enumerable of the stories in the session.
        /// </summary>
        [Required]
        [MinLength(1)]
        public IEnumerable<StoryModel> Stories { get; set; }

        /// <summary>
        /// Host nickname.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Nickname { get; set; }

        /// <summary>
        /// Point that users can select in the session.
        /// </summary>
        public IEnumerable<string> PointChoices { get; set; } = new string[] { "0", "½", "1", "2", "3", "5", "8", "13", "20", "40", "100", "∞", "?" };

        /// <summary>
        /// Host User ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Host actions authentication code.
        /// </summary>
        public string HostCode { get; set; }
        
        /// <summary>
        /// Session ID.
        /// </summary>
        public string SessionId { get; set; }
    }
}