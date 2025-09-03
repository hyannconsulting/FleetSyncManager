namespace Laroche.FleetManager.Application.Interfaces
{
    /// <summary>
    /// Defines a contract for generating tokens based on user information.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for creating tokens that represent the
    /// specified user. The generated token can be used for authentication, authorization, or other purposes as defined
    /// by the application.</remarks>
    public interface ITokenProvider
    {
        /// <summary>
        /// Generates a JSON Web Token (JWT) for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is being created. Must contain valid user information.</param>
        /// <returns>A string representing the generated JWT.</returns>
        string CreateToken(DTOs.Users.UserDto user);
    }
}
