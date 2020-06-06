using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CardsForProductivity.API.Models.Api;
using CardsForProductivity.API.Providers;
using CardsForProductivity.API.Models.Data;
using CardsForProductivity.API.Helpers;
using Microsoft.AspNetCore.Http;

namespace CardsForProductivity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        readonly ISessionProvider _sessionProvider;

        public SessionController(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        /// <summary>
        /// Allows a user to join a session.
        /// </summary>
        /// <param name="joinSessionRequest">Join session request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("join")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> JoinSessionAsync(JoinSessionRequest joinSessionRequest, CancellationToken cancellationToken)
        {
            var session = await _sessionProvider.GetSessionBySessionCodeAsync(joinSessionRequest.SessionCode, cancellationToken);

            if (session is null)
            {
                return NotFound();
            }

            var userJoinCheck = await CheckUserCanJoinSessionAsync(session, joinSessionRequest.Nickname, joinSessionRequest.UserId, joinSessionRequest.RejoinCode, cancellationToken);

            if (userJoinCheck.GetType() != typeof(OkResult))
            {
                return userJoinCheck;
            }

            var response = await _sessionProvider.JoinSessionAsync(joinSessionRequest, session.SessionId, cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Creates a session.
        /// </summary>
        /// <param name="createSessionRequest">Create session request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSessionAsync(CreateSessionRequest createSessionRequest, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(createSessionRequest.HostCode) && !string.IsNullOrEmpty(createSessionRequest.SessionId))
            {
                var session = await _sessionProvider.GetSessionByIdAsync(createSessionRequest.SessionId, cancellationToken);

                var userJoinCheck = await CheckUserCanJoinSessionAsync(session, createSessionRequest.Nickname, createSessionRequest.UserId, createSessionRequest.HostCode, cancellationToken);

                if (userJoinCheck.GetType() != typeof(OkResult))
                {
                    return userJoinCheck;
                }
            }

            var response = await _sessionProvider.CreateSessionAsync(createSessionRequest, cancellationToken);

            return Ok(response);
        }

        async Task<IActionResult> CheckUserCanJoinSessionAsync(SessionModel session, string nickname, string userId, string rejoinCode, CancellationToken cancellationToken)
        {
            if (await _sessionProvider.DoesUserExistInSessionAsync(session.SessionId, nickname, cancellationToken))
            {
                if (userId != null && !ValidationHelper.ValidateObjectId(userId))
                {
                    return BadRequest();
                }

                var user = string.IsNullOrEmpty(userId) ? null : await _sessionProvider.GetUserByIdAsync(userId, cancellationToken);

                if (user is null || user.RejoinCode != rejoinCode)
                {
                    return Conflict();
                }
            }

            return Ok();
        }
    }
}
