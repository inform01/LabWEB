using Crypto.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Authorize(PolicyNames.Admin)]
public class CurrenciesController : Controller
{
    private readonly CryptoDbContext _context;

    public CurrenciesController(CryptoDbContext context)
    {
        _context = context;
    }

    // GET: Currencies
    public async Task<IActionResult> Index()
    {
        return View(await _context.Currencies.ToListAsync());
    }

    // GET: Currencies/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Currencies == null)
        {
            return NotFound();
        }

        var cryptoCurrency = await _context.Currencies
            .FirstOrDefaultAsync(m => m.Id == id);
        if (cryptoCurrency == null)
        {
            return NotFound();
        }

        return View(cryptoCurrency);
    }

    // GET: Currencies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Currencies/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] CryptoCurrency cryptoCurrency)
    {
        if (ModelState.IsValid)
        {
            _context.Add(cryptoCurrency);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(cryptoCurrency);
    }

    // GET: Currencies/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Currencies == null)
        {
            return NotFound();
        }

        var cryptoCurrency = await _context.Currencies.FindAsync(id);
        if (cryptoCurrency == null)
        {
            return NotFound();
        }
        return View(cryptoCurrency);
    }

    // POST: Currencies/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,Id")] CryptoCurrency cryptoCurrency)
    {
        if (id != cryptoCurrency.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(cryptoCurrency);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CryptoCurrencyExists(cryptoCurrency.Id))
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
        return View(cryptoCurrency);
    }

    // GET: Currencies/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Currencies == null)
        {
            return NotFound();
        }

        var cryptoCurrency = await _context.Currencies
            .FirstOrDefaultAsync(m => m.Id == id);
        if (cryptoCurrency == null)
        {
            return NotFound();
        }

        return View(cryptoCurrency);
    }

    // POST: Currencies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cryptoCurrency = await _context.Currencies.FindAsync(id);
        if (cryptoCurrency != null)
        {
            _context.Currencies.Remove(cryptoCurrency);
        }
            
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CryptoCurrencyExists(int id)
    {
        return _context.Currencies.Any(e => e.Id == id);
    }
}
