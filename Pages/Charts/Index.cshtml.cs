using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Pages.Charts;

public class ChartsPageModel : PageModel
{
    private readonly CryptoDbContext _context;

    public ICollection<PairsInfo> PairsInfos { get; private set; }
    
    public ICollection<ExchangeInfo> ExchangeInfos { get; private set; }

    public class PairsInfo
    {
        public string PairName { get; set; }
        public int ExchangesCount { get; set; }
    }

    public class ExchangeInfo
    {
        public string ExchangeName { get; set; }
        public decimal? AveragePairPrice { get; set; }
    }
    
    public ChartsPageModel(CryptoDbContext context)
    {
        _context = context;
    }
    
    public async Task OnGet()
    {
        PairsInfos = await _context.Pairs
            .Select(x => new PairsInfo
            {
                PairName = x.FirstCrypto.Name + " -> " + x.SecondCrypto.Name,
                ExchangesCount = x.Exchanges.Count
            })
            .ToListAsync();

        ExchangeInfos = await _context.Exchanges
            .Select(x => new ExchangeInfo
            {
                ExchangeName = x.Name,
                AveragePairPrice = x.ExchangePairs.Average(y => y.Price)
            })
            .ToListAsync();
    }
}
