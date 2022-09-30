using System.ComponentModel.DataAnnotations;
using Crypto.Extensions;
using Crypto.Models;
using Crypto.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Pages.Orders;

public class Create : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int ExchangePairId { get; set; }
    
    [BindProperty]
    public OrderCreateModel CreateOrderModel { get; set; }

    public ExchangePairInfoModel PairInfoModel { get; set; }
    
    private readonly CryptoDbContext _context;
    
    public Create(CryptoDbContext context)
    {
        _context = context;
    }

    public async Task OnGet()
    {
        await LoadPairInfo();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            await LoadPairInfo();
            return Page();
        }

        var order = new Order
        {
            CustomerId = User.GetCustomerId(),
            ExchangePairId = ExchangePairId,
            Volume = CreateOrderModel.Volume,
            Date = DateTime.Now
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "PairsIndex");
    }

    private async Task LoadPairInfo()
    {
        PairInfoModel = await _context.ExchangePairs
            .Where(x => x.Id == ExchangePairId)
            .Select(x => new ExchangePairInfoModel
            {
                Price = x.Price,
                PairName = x.Pair.FirstCrypto.Name + " -> " + x.Pair.SecondCrypto.Name,
                ExchangeName = x.Exchange.Name
            })
            .FirstOrDefaultAsync();

        if (PairInfoModel is null)
        {
            throw new NotFoundException<ExchangePair>();
        }
    }
    
    public class ExchangePairInfoModel
    {
        public string ExchangeName { get; set; }
        public string PairName { get; set; }
        public decimal Price { get; set; }
    }
    
    public class OrderCreateModel
    {
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Volume { get; set; }
    }
}
