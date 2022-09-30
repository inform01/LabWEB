using Crypto.Models.Dtos.ExchangePairs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Authorize]
public class PairsIndexController : Controller
{
    private readonly CryptoDbContext _context;
        
    public PairsIndexController(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var dtos = await _context.ExchangePairs
            .OrderBy(x => x.PairId)
            .ThenBy(x => x.Exchange.Name)
            .Select(ExchangePairIndexDto.Mapper)
            .ToListAsync();
        return View(dtos);
    }
}
