namespace CardsForProductivity.API.Models.Api
{
    /// <summary>
    /// Client request details.
    /// </summary>
    public class ClientRequestDetails
    {
        /// <summary>
        /// Session ID.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Session code.
        /// </summary>
        public string SessionCode { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Authentication code.
        /// </summary>
        public string AuthCode { get; set; }
    }
}