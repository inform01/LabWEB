using System.Linq.Expressions;
using Crypto.Models.Entities;

namespace Crypto.Models.Dtos.Pairs;

public class PairShortDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public static Expression<Func<Pair, PairShortDto>> Mapper { get; } =
        x => new PairShortDto()
        {
            Id = x.Id,
            Name = x.FirstCrypto.Name + " -> " + x.SecondCrypto.Name
        };
}
