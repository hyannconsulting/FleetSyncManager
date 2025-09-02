using Microsoft.AspNetCore.Identity;
using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Infrastructure.Services;

/// <summary>
/// Service de test pour UserManager
/// </summary>
public class TestUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public TestUserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetUserAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }
}
