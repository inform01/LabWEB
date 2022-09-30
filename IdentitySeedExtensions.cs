using Crypto.Models.Entities;
using Crypto.Models.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Crypto;

public static class IdentitySeedExtensions
{
    public static async Task EnsureIdentityInitialized(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var authContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await authContext.Database.MigrateAsync();
        
        if (!await authContext.Users.AnyAsync(x => x.Id == AdminConsts.Id))
        {
            var user = new ApplicationUser
            {
                Id = AdminConsts.Id,
                Email = AdminConsts.Email,
                UserName = AdminConsts.UserName,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, AdminConsts.DefaultPassword);
        }

        await using (var dbContext = scope.ServiceProvider.GetRequiredService<CryptoDbContext>())
        {
            if (!await dbContext.Customers.AnyAsync(x => x.IdentityUserId == AdminConsts.Id))
            {
                var adminCustomer = new Customer
                {
                    IdentityUserId = AdminConsts.Id,
                    Name = "AdminCustomer",
                    DateOfBirth = DateTime.Now.AddYears(-19)
                };
                dbContext.Customers.Add(adminCustomer);
                await dbContext.SaveChangesAsync();
            }
        }

        if (!await authContext.Roles.AnyAsync(x => x.Name == RoleNames.Admin))
        {
            authContext.Roles.Add(new(RoleNames.Admin)
            {
                Id = Guid.NewGuid().ToString(),
                NormalizedName = RoleNames.Admin.ToUpperInvariant()
            });
            await authContext.SaveChangesAsync();
        }


        var adminRole = await authContext.Roles.FirstAsync(x => x.Name == RoleNames.Admin);
        if (!await authContext.UserRoles.AnyAsync(x => x.RoleId == adminRole.Id && x.UserId == AdminConsts.Id))
        {
            authContext.UserRoles.Add(new()
            {
                UserId = AdminConsts.Id,
                RoleId = adminRole.Id
            });
            await authContext.SaveChangesAsync();
        }
        
        
    }
}
