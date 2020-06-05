using System.ComponentModel.DataAnnotations;

namespace CardsForProductivity.API.Models.Api
{
    /// <summary>
    /// Join session request.
    /// </summary>
    public class JoinSessionRequest
    {
        /// <summary>
        /// Session ID to join.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Session code to authenticate the join request with.
        /// </summary>
        [Required]
        public string SessionCode { get; set; }

        /// <summary>
        /// Nickname.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Nickname { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Rejoin code.
        /// </summary>
        public string RejoinCode { get; set; }
    }
}