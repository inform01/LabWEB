using Crypto.Extensions;
using Crypto.Models.Dtos.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly CryptoDbContext _context;

    public OrdersController(CryptoDbContext context)
    {
        _context = context;
    }

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        var id = User.GetCustomerId();
        var dtos = await _context.Orders
            .Where(x => x.CustomerId == id)
            .Select(OrderDto.Mapper)
            .ToListAsync();
        return View(dtos);
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }

        var customerId = User.GetCustomerId();
        var order = await _context.Orders
            .Where(x => x.Id == id && x.CustomerId == customerId)
            .Select(OrderDto.Mapper)
            .FirstOrDefaultAsync();
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}
