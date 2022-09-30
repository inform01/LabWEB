namespace Crypto.Models.Entities;

public class ExchangePair : BaseEntity
{
    public int ExchangeId { get; set; }
    public Exchange Exchange { get; set; }
    
    public int PairId { get; set; }
    public Pair Pair { get; set; }
    
    public decimal Price { get; set; }
}
