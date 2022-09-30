using System.Security.Claims;
using Crypto.Models.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Crypto.Services.Identity.Implementations;

public class CryptoClaimsIdentityFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    private readonly CryptoDbContext _context;

    public CryptoClaimsIdentityFactory(
        IOptions<IdentityOptions> optionsAccessor,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        CryptoDbContext context) : base(userManager, roleManager, optionsAccessor)
    {
        _context = context;
    }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        var claimsIdentity = (ClaimsIdentity) principal.Identity!;
        
        var customerId = await _context.Customers
            .Where(x => x.IdentityUserId == user.Id)
            .Select(x => x.Id)
            .FirstAsync();
        claimsIdentity.AddClaim(new(CryptoClaims.CustomerId, customerId.ToString()));

        return principal;
    }
}
