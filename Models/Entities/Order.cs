namespace Crypto.Models.Entities;

public class Order : BaseEntity
{
    public DateTime Date { get; set; }
    public decimal Volume { get; set; }

    public int ExchangePairId { get; set; }
    public ExchangePair ExchangePair { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}
