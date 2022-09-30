namespace Crypto.Models.Dtos.Users;

public class UserReadDto
{
    public string Id { get; init; }
    
    public string UserName { get; init; }

    private UserReadDto() {}

    public UserReadDto(
        string id,
        string userName
    )
    {
        Id = id;
        UserName = userName;
    }
}
