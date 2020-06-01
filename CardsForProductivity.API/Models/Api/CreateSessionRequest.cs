using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CardsForProductivity.API.Models.Data;

namespace CardsForProductivity.API.Models.Api
{
    public class CreateSessionRequest
    {
        [Required]
        [MinLength(1)]
        public IEnumerable<StoryModel> Stories { get; set; }

        [Required]
        [MaxLength(20)]
        public string Nickname { get; set; }

        public IEnumerable<string> PointChoices { get; set; } = new string[] { "0", "½", "1", "2", "3", "5", "8", "13", "20", "40", "100", "∞", "?" };

        public string UserId { get; set; }

        public string HostCode { get; set; }

        public string SessionId { get; set; }
    }
}