using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.Application.DTOs;
using Product.Application.Interfaces;

namespace Product.Api.Controllers.v1
{
    /// <summary>
    /// Handles user authentication, registration, token refresh, and revocation.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The registration details (username, password, role).</param>
        /// <returns>A token response with the access and refresh tokens.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">If the username is already taken or the input request is invalid.</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Authenticates an existing user and issues tokens.
        /// </summary>
        /// <param name="request">The user login credentials.</param>
        /// <returns>A token response containing the access and refresh tokens.</returns>
        /// <response code="200">Login successful, returns authorization tokens.</response>
        /// <response code="400">If username or password is incorrect, or validation fails.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Refreshes an expired JWT access token using a valid refresh token.
        /// </summary>
        /// <param name="request">The current expired access token and valid refresh token.</param>
        /// <returns>A new set of access and refresh tokens.</returns>
        /// <response code="200">Tokens refreshed successfully.</response>
        /// <response code="400">If the refresh token is expired, invalid, or reused.</response>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Revokes a refresh token, logging the user out.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Token revoked successfully.</response>
        /// <response code="400">If the token is invalid or already revoked.</response>
        /// <response code="401">If the caller is unauthorized.</response>
        [HttpPost("revoke")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Revoke([FromBody] string refreshToken)
        {
            await _authService.RevokeTokenAsync(refreshToken);
            return NoContent();
        }
    }
}
