using Crypto.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Authorize(PolicyNames.Admin)]
public class PairsController : Controller
{
    private readonly CryptoDbContext _context;

    public PairsController(CryptoDbContext context)
    {
        _context = context;
    }

    // GET: Pairs
    public async Task<IActionResult> Index()
    {
        var cryptoDbContext = _context.Pairs.Include(p => p.FirstCrypto).Include(p => p.SecondCrypto);
        return View(await cryptoDbContext.ToListAsync());
    }

    // GET: Pairs/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Pairs == null)
        {
            return NotFound();
        }

        var pair = await _context.Pairs
            .Include(p => p.FirstCrypto)
            .Include(p => p.SecondCrypto)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (pair == null)
        {
            return NotFound();
        }

        return View(pair);
    }

    // GET: Pairs/Create
    public IActionResult Create()
    {
        ViewData["FirstCryptoId"] = new SelectList(_context.Currencies, "Id", "Name");
        ViewData["SecondCryptoId"] = new SelectList(_context.Currencies, "Id", "Name");
        return View();
    }

    // POST: Pairs/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstCryptoId,SecondCryptoId,Id")] Pair pair)
    {
        if (ModelState.IsValid)
        {
            if (pair.FirstCryptoId == pair.SecondCryptoId)
            {
                ModelState.AddModelError(string.Empty, "Invalid pair");
                ViewData["FirstCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.FirstCryptoId);
                ViewData["SecondCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.SecondCryptoId);
                return View(pair);
            }
            if (await _context.Pairs.AnyAsync(x => x.FirstCryptoId == pair.FirstCryptoId && x.SecondCryptoId == pair.SecondCryptoId))
            {
                ModelState.AddModelError(string.Empty, "This pair already exists");
                ViewData["FirstCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.FirstCryptoId);
                ViewData["SecondCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.SecondCryptoId);
                return View(pair);
            }
            _context.Add(pair);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["FirstCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.FirstCryptoId);
        ViewData["SecondCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.SecondCryptoId);
        return View(pair);
    }

    // GET: Pairs/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Pairs == null)
        {
            return NotFound();
        }

        var pair = await _context.Pairs.FindAsync(id);
        if (pair == null)
        {
            return NotFound();
        }
        ViewData["FirstCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.FirstCryptoId);
        ViewData["SecondCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.SecondCryptoId);
        return View(pair);
    }

    // POST: Pairs/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("FirstCryptoId,SecondCryptoId,Id")] Pair pair)
    {
        if (id != pair.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (pair.FirstCryptoId == pair.SecondCryptoId)
                {
                    ModelState.AddModelError(string.Empty, "Invalid pair");
                    ViewData["FirstCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.FirstCryptoId);
                    ViewData["SecondCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.SecondCryptoId);
                    return View(pair);
                }
                if (await _context.Pairs.AnyAsync(x => x.Id != id && x.FirstCryptoId == pair.FirstCryptoId && x.SecondCryptoId == pair.SecondCryptoId))
                {
                    ModelState.AddModelError(string.Empty, "This pair already exists");
                    ViewData["FirstCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.FirstCryptoId);
                    ViewData["SecondCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.SecondCryptoId);
                    return View(pair);
                }
                    
                _context.Update(pair);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PairExists(pair.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["FirstCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.FirstCryptoId);
        ViewData["SecondCryptoId"] = new SelectList(_context.Currencies, "Id", "Name", pair.SecondCryptoId);
        return View(pair);
    }

    // GET: Pairs/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Pairs == null)
        {
            return NotFound();
        }

        var pair = await _context.Pairs
            .Include(p => p.FirstCrypto)
            .Include(p => p.SecondCrypto)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (pair == null)
        {
            return NotFound();
        }

        return View(pair);
    }

    // POST: Pairs/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Pairs == null)
        {
            return Problem("Entity set 'CryptoDbContext.Pairs'  is null.");
        }
        var pair = await _context.Pairs.FindAsync(id);
        if (pair != null)
        {
            _context.Pairs.Remove(pair);
        }
            
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PairExists(int id)
    {
        return _context.Pairs.Any(e => e.Id == id);
    }
}
