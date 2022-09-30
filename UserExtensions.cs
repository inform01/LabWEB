using System.Security.Claims;
using Crypto.Models.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Crypto;

public static class UserExtensions
{
    public static bool IsAdmin(this SignInManager<ApplicationUser> manager, ClaimsPrincipal user)
    {
        return manager.IsSignedIn(user) && user.IsInRole(RoleNames.Admin);
    }
}
