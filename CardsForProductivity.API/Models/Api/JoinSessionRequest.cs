namespace CardsForProductivity.API.Models.Api
{
    public class JoinSessionRequest
    {
        public string SessionId { get; set; }

        public string SessionCode { get; set; }

        public string Nickname { get; set; }

        public string UserId { get; set; }

        public string RejoinCode { get; set; }
    }
}