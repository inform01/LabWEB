using Crypto.Models.Dtos.Exchanges;
using Crypto.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Authorize(PolicyNames.Admin)]
public class ExchangesController : Controller
{
    private readonly CryptoDbContext _context;

    public ExchangesController(CryptoDbContext context)
    {
        _context = context;
    }

    // GET: Exchanges
    public async Task<IActionResult> Index()
    {
        var dtos = await _context.Exchanges.Select(x => new ExchangeTableDto
            {
                Id = x.Id,
                Name = x.Name,
                PairsCount = x.Pairs.Count,
                RegistrationDate = x.RegistrationDate
            })
            .ToListAsync();
        return View(dtos);
    }

    // GET: Exchanges/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Exchanges == null)
        {
            return NotFound();
        }

        var exchange = await _context.Exchanges
            .FirstOrDefaultAsync(m => m.Id == id);
        if (exchange == null)
        {
            return NotFound();
        }

        return View(exchange);
    }

    // GET: Exchanges/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Exchanges/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,RegistrationDate,Id")] Exchange exchange)
    {
        if (!ModelState.IsValid)
            return View(exchange);

        if (await _context.Exchanges.AnyAsync(x => x.Name == exchange.Name))
        {
            ModelState.AddModelError(string.Empty, "Exchange already exists");
            return View(exchange);
        }

        _context.Add(exchange);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Exchanges/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Exchanges == null)
        {
            return NotFound();
        }

        var exchange = await _context.Exchanges.FindAsync(id);
        if (exchange == null)
        {
            return NotFound();
        }
        return View(exchange);
    }

    // POST: Exchanges/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,RegistrationDate,Id")] Exchange exchange)
    {
        if (id != exchange.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
            return View(exchange);
            
        if (await _context.Exchanges.AnyAsync(x => x.Id != id && x.Name == exchange.Name))
        {
            ModelState.AddModelError(string.Empty, "Exchange already exists");
            return View(exchange);
        }
            
        try
        {
            _context.Update(exchange);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExchangeExists(exchange.Id))
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

    // GET: Exchanges/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Exchanges == null)
        {
            return NotFound();
        }

        var exchange = await _context.Exchanges
            .FirstOrDefaultAsync(m => m.Id == id);
        if (exchange == null)
        {
            return NotFound();
        }

        return View(exchange);
    }

    // POST: Exchanges/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var exchange = await _context.Exchanges.FindAsync(id);
        if (exchange != null)
        {
            _context.Exchanges.Remove(exchange);
        }
            
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ExchangeExists(int id)
    {
        return _context.Exchanges.Any(e => e.Id == id);
    }
}
