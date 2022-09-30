namespace Crypto.Models.Entities;

public class Exchange : BaseEntity
{
    public string Name { get; set; }
    public DateTime RegistrationDate { get; set; }

    public ICollection<ExchangePair> ExchangePairs { get; set; } = new List<ExchangePair>();
    public ICollection<Pair> Pairs { get; set; } = new List<Pair>();
}
