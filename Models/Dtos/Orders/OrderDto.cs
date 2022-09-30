using System.Linq.Expressions;
using Crypto.Models.Entities;

namespace Crypto.Models.Dtos.Orders;

public class OrderDto
{
    public int Id { get; set; }
    public string PairName{ get; set; }
    public string ExchangeName { get; set; }
    public decimal Price { get; set; }
    public decimal Volume { get; set; }
    public DateTime Date { get; set; }

    public static Expression<Func<Order, OrderDto>> Mapper { get; } = x => new OrderDto
    {
        Id = x.Id,
        Price = x.ExchangePair.Price,
        Volume = x.Volume,
        PairName = x.ExchangePair.Pair.FirstCrypto.Name + " -> " + x.ExchangePair.Pair.SecondCrypto.Name,
        ExchangeName = x.ExchangePair.Exchange.Name,
        Date = x.Date
    };
}
