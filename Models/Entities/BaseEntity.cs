using System.ComponentModel.DataAnnotations;

namespace Crypto.Models.Entities;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
