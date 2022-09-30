using System.Linq.Expressions;
using Crypto.Models.Entities;

namespace Crypto.Models.Dtos.ExchangePairs;

public class ExchangePairIndexDto
{
    public int Id { get; set; }
    public string Pair { get; set; }
    public decimal Price { get; set; }
    public int ExchangeId { get; set; }
    public string ExchangeName { get; set; }

    public static Expression<Func<ExchangePair, ExchangePairIndexDto>> Mapper { get; } = x => new()
    {
        Id = x.Id,
        Price = x.Price,
        Pair = x.Pair.FirstCrypto.Name + " -> " + x.Pair.SecondCrypto.Name,
        ExchangeId = x.ExchangeId,
        ExchangeName = x.Exchange.Name
    };
}
