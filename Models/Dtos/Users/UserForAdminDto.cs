namespace Crypto.Models.Dtos.Users;

public record UserForAdminDto(
    string Id,
    string UserName,
    string Email,
    ICollection<string> Roles
);
