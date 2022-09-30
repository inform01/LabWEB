using System.ComponentModel.DataAnnotations;

namespace Crypto.Models.Dtos.Users;

public class UserWriteDto
{
    [Required]
    public string UserName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public bool IsAdmin { get; set; } = false;

    public UserWriteDto()
    {
        UserName = string.Empty;
    }

    public UserWriteDto(string userName)
    {
        UserName = userName;
    }
}
