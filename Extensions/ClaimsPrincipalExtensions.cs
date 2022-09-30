using System.Security.Claims;

namespace Crypto.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetCustomerId(this ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirstValue(CryptoClaims.CustomerId));
    }

    public static Guid GetId(this ClaimsPrincipal user)
    {
        return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
    }
    
    public static Guid? FindId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return value is null ? null : Guid.Parse(value);
    }
}
