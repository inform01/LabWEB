namespace Crypto.Models.Entities;

public class Customer : BaseEntity
{
    public string IdentityUserId { get; set; }
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
