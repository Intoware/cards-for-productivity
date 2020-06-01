using System.ComponentModel.DataAnnotations;

namespace CardsForProductivity.API.Models.Api
{
    public class JoinSessionRequest
    {
        [Required]
        public string SessionId { get; set; }

        [Required]
        public string SessionCode { get; set; }

        [Required]
        [MaxLength(20)]
        public string Nickname { get; set; }

        public string UserId { get; set; }

        public string RejoinCode { get; set; }
    }
}