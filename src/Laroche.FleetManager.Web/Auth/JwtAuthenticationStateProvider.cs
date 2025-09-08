using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Laroche.FleetManager.Web.Auth;

/// <summary>
/// Provides an implementation of <see cref="AuthenticationStateProvider"/> that uses JSON Web Tokens (JWT) to manage
/// and retrieve the authentication state of the user.
/// </summary>
/// <remarks>This class interacts with the browser's local storage to retrieve and manage the user's
/// authentication state. It assumes that a JWT access token is stored under the key "access_token" in local storage.
/// The token is used to create a claims-based identity for the user. If the token is missing or invalid, the user is
/// considered unauthenticated, and an anonymous identity is returned.</remarks>
/// <param name="jsRuntime"></param>
public class JwtAuthenticationStateProvider(IJSRuntime jsRuntime) : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private ClaimsPrincipal _anonymous => new(new ClaimsIdentity());

    /// <summary>
    /// Asynchronously retrieves the current authentication state of the user.
    /// </summary>
    /// <remarks>This method checks for a JWT access token stored in the browser's local storage under the key
    /// "access_token". If the token is not found or is empty, the user is considered unauthenticated, and an anonymous
    /// authentication state is returned. Otherwise, the token is parsed to extract claims, and an authentication state
    /// is created with a claims-based identity.</remarks>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the  <see
    /// cref="AuthenticationState"/> of the user, which includes either an anonymous identity or a claims-based
    /// identity.</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "access_token");
            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(_anonymous);

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (InvalidOperationException)
        {
            // Appel JSInterop lors du prerendering : retourne anonyme
            return new AuthenticationState(_anonymous);
        }
    }

    /// <summary>
    /// Notifies the application of a user's authentication state based on the provided JWT token.
    /// </summary>
    /// <remarks>This method updates the authentication state by creating a <see cref="ClaimsPrincipal"/> from
    /// the parsed claims in the provided token. The authentication state change is propagated to subscribers of the
    /// authentication state.</remarks>
    /// <param name="token">The JSON Web Token (JWT) used to authenticate the user. Must be a valid token containing the necessary claims.</param>
    public void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    /// <summary>
    /// Notifies the application that the user has logged out, updating the authentication state accordingly.
    /// </summary>
    /// <remarks>This method triggers an authentication state change event, setting the user to an anonymous
    /// state. It can be used to ensure that the application reflects the user's logged-out status.</remarks>
    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}
