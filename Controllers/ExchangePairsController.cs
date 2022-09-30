using Crypto.Models;
using Crypto.Models.Dtos.ExchangePairs;
using Crypto.Models.Dtos.Pairs;
using Crypto.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Authorize(PolicyNames.Admin)]
public class ExchangePairsController : Controller
{
    [FromRoute]
    public int ExchangeId { get; set; }
        
    private readonly CryptoDbContext _context;

    public ExchangePairsController(CryptoDbContext context)
    {
        _context = context;
    }

    // GET: ExchangePairs
    public async Task<IActionResult> Index()
    {
        await LoadExchangeData();
        var dtos = _context.ExchangePairs
            .Where(x => x.ExchangeId == ExchangeId)
            .Select(x => new ExchangePairTableDto
            {
                Id = x.Id,
                Price = x.Price,
                Pair = x.Pair.FirstCrypto.Name + " -> " + x.Pair.SecondCrypto.Name
            });
        return View(dtos);
    }

    // GET: ExchangePairs/Create
    public async Task<IActionResult> Create()
    {
        await LoadExchangeData();
        await LoadPairs();
        return View();
    }

    // POST: ExchangePairs/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PairId,Price")] ExchangePair exchangePair)
    {
        if (!ModelState.IsValid)
        {
            await LoadExchangeData();
            await LoadPairs();
            return View(exchangePair);
        }

        if (await _context.ExchangePairs.AnyAsync(x => x.ExchangeId == ExchangeId && x.PairId == exchangePair.PairId))
        {
            ModelState.AddModelError(string.Empty, "Pair already exists for this exchange");
            await LoadExchangeData();
            await LoadPairs();
            return View(exchangePair);
        }
            
        exchangePair.ExchangeId = ExchangeId;
        _context.Add(exchangePair);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: ExchangePairs/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.ExchangePairs == null)
        {
            return NotFound();
        }

        var exchangePair = await _context.ExchangePairs.FindAsync(id);
        if (exchangePair == null)
        {
            return NotFound();
        }
        await LoadExchangeData();
        await LoadPairs();
        return View(exchangePair);
    }

    // POST: ExchangePairs/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PairId,Price,Id")] ExchangePair exchangePair)
    {
        if (id != exchangePair.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadExchangeData();
            await LoadPairs();
            return View(exchangePair);
        }

        try
        {
            if (await _context.ExchangePairs.AnyAsync(x => x.Id != id && x.ExchangeId == ExchangeId && x.PairId == exchangePair.PairId))
            {
                ModelState.AddModelError(string.Empty, "Pair already exists for this exchange");
                await LoadExchangeData();
                await LoadPairs();
                return View(exchangePair);
            }
                
            exchangePair.ExchangeId = ExchangeId;
            _context.Update(exchangePair);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExchangePairExists(exchangePair.Id))
            {
                return NotFound();
            }
            throw;
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: ExchangePairs/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.ExchangePairs == null)
        {
            return NotFound();
        }

        var exchangePair = await _context.ExchangePairs
            .Include(e => e.Exchange)
            .Include(e => e.Pair)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (exchangePair == null)
        {
            return NotFound();
        }

        return View(exchangePair);
    }

    // POST: ExchangePairs/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var exchangePair = await _context.ExchangePairs.FindAsync(id);
        if (exchangePair != null)
        {
            _context.ExchangePairs.Remove(exchangePair);
        }
            
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadExchangeData()
    {
        ViewBag.Exchange = await _context.Exchanges.FirstOrDefaultAsync(x => x.Id == ExchangeId);
        if (ViewBag.Exchange is null)
        {
            throw new NotFoundException<Exchange>();
        }
    }

    private async Task LoadPairs()
    {
        var pairDtos = await _context.Pairs.Select(PairShortDto.Mapper).ToListAsync();
        ViewBag.Pairs = new SelectList(pairDtos, "Id", "Name");
    }

    private bool ExchangePairExists(int id)
    {
        return _context.ExchangePairs.Any(e => e.Id == id);
    }
}
