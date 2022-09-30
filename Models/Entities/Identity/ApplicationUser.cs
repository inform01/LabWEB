using Microsoft.AspNetCore.Identity;

namespace Crypto.Models.Entities.Identity;

public class ApplicationUser : IdentityUser<string>
{
    public ICollection<ApplicationRole> Roles { get; set; } = new HashSet<ApplicationRole>();
}
