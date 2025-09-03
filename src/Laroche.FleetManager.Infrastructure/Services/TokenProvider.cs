using Laroche.FleetManager.Application.DTOs.Users;
using Laroche.FleetManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Laroche.FleetManager.Infrastructure.Services
{
    /// <summary>
    /// Provides functionality for creating JSON Web Tokens (JWT) based on user information and application
    /// configuration.
    /// </summary>
    /// <remarks>This class is designed to generate JWTs using the settings specified in the application's
    /// configuration. The configuration must include the following keys under the "JwtSettings" section: <list
    /// type="bullet"> <item><term>SecretKey</term>: The secret key used to sign the token.</item>
    /// <item><term>Issuer</term>: The issuer of the token.</item> <item><term>Audience</term>: The intended audience of
    /// the token.</item> <item><term>ExpiryInMinutes</term>: The token's expiration time in minutes.</item>
    /// </list></remarks>
    /// <param name="configuration"></param>
    public sealed class TokenProvider(IConfiguration configuration) : ITokenProvider
    {
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Generates a JSON Web Token (JWT) for the specified user.
        /// </summary>
        /// <remarks>The token includes claims such as the user's ID, email, username, first name, last
        /// name, active status, and roles. The token's expiration time, issuer, audience, and signing key are configured
        /// using the application's settings.</remarks>
        /// <param name="user">The user for whom the token is being created. The user's details are included as claims in the token.</param>
        /// <returns>A string representation of the generated JWT.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the JWT secret key is not configured in the application settings.</exception>
        public string CreateToken(UserDto user)
        {

            var tokenKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("id", user.Id),
                    new System.Security.Claims.Claim("email", user.Email),
                    new System.Security.Claims.Claim("username", user.UserName),
                    new System.Security.Claims.Claim("firstName", user.FirstName),
                    new System.Security.Claims.Claim("lastName", user.LastName),
                    new System.Security.Claims.Claim("isActive", user.IsActive.ToString()),
                    new System.Security.Claims.Claim("roles", string.Join(",", user.Roles))
                }),
                Expires =
                    DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationMinutes")),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
