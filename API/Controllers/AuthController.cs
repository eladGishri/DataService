using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataService.Api.Controllers
{
    /// <summary>
    /// Authentication controller that provides JWT token-based authentication for the DataService API.
    /// Handles user login and JWT token generation for authorized access to protected endpoints.
    /// </summary>
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Hardcoded user credentials for authentication.
        /// Contains username, password, and role information for demo users.
        /// </summary>
        private static readonly List<(string Username, string Password, string Role)> Users = new()
        {
            ("admin", "admin123", "Admin"),
            ("user", "user123", "User")
        };

        /// <summary>
        /// Authenticates a user and returns a JWT token for accessing protected API endpoints.
        /// Validates credentials against hardcoded user list and generates a signed JWT token on success.
        /// </summary>
        /// <param name="request">The login request containing username and password credentials.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing:
        /// - 200 OK with JWT token if authentication succeeds
        /// - 401 Unauthorized if credentials are invalid
        /// </returns>
        /// <response code="200">Authentication successful, returns JWT token</response>
        /// <response code="400">Invalid request format or missing credentials</response>
        /// <response code="401">Invalid username or password</response>
        /// <example>
        /// POST /auth/login
        /// {
        ///   "username": "admin",
        ///   "password": "admin123"
        /// }
        /// 
        /// Response:
        /// {
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// }
        /// </example>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validate user credentials against hardcoded user list
            var user = Users.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
            if (user == default)
                return Unauthorized();

            // Create JWT claims with user information
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Configure JWT token parameters
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EGsupersecretkeyofAuthentication"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Generate JWT token with 1-hour expiration
            var token = new JwtSecurityToken(
                issuer: "DataServiceApi",
                audience: "DataServiceApi",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            // Return the serialized token
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        /// <summary>
        /// Represents a user login request containing authentication credentials.
        /// Used as the request body for the login endpoint.
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// Gets or sets the username for authentication.
            /// Must match one of the configured user accounts.
            /// </summary>
            /// <value>A string representing the user's login name.</value>
            /// <example>admin</example>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the password for authentication.
            /// Must match the password associated with the specified username.
            /// </summary>
            /// <value>A string representing the user's password.</value>
            /// <example>admin123</example>
            public string Password { get; set; }
        }
    }
}