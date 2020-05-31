using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CardsForProductivity.API.Models.Api;
using CardsForProductivity.API.Providers;
using CardsForProductivity.API.Models.Data;
using CardsForProductivity.API.Helpers;

namespace CardsForProductivity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        readonly ILogger<SessionController> _logger;
        readonly ISessionProvider _sessionProvider;

        public SessionController(ILogger<SessionController> logger,
            ISessionProvider sessionProvider)
        {
            _logger = logger;
            _sessionProvider = sessionProvider;
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinSessionAsync([FromBody] JoinSessionRequest joinSessionRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var session = await _sessionProvider.GetSessionBySessionCodeAsync(joinSessionRequest.SessionCode, cancellationToken);

            if (session == null)
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

        [HttpPost]
        public async Task<IActionResult> CreateSessionAsync([FromBody] CreateSessionRequest createSessionRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

                if (user == null || user.RejoinCode != rejoinCode)
                {
                    return Conflict();
                }
            }

            return Ok();
        }
    }
}
