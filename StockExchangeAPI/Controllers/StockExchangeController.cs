using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockExchangeAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace StockExchangeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StockExchangeController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;
        private readonly StockExchangeDBContext _context;
        private readonly ILogger<StockExchangeController> _logger;

        public StockExchangeController(IStockRepository stockRepo, StockExchangeDBContext context, ILogger<StockExchangeController> logger)
        {
            _stockRepo = stockRepo;
            _context = context;
            _logger = logger;
        }

        [HttpGet("api/stocks", Name = "GetStocks")]
        public IEnumerable<Stock> Get()
        {
            return _stockRepo.GetStocks();
        }

        [HttpPost("api/subscribe", Name = "AddStock")]
        public async Task<IActionResult> SubscribeToStock(string stockSymbol)
        {
            var stock = _stockRepo.GetStocks().FirstOrDefault(s => s.StockSymbol == stockSymbol);

            if (stock == null)
            {
                var newStock = new Stock
                {
                    StockSymbol = stockSymbol,
                    Price = Random.Shared.Next(100, 200),
                    TimeStamp = DateTime.Now
                };
                _context.Stocks.Add(newStock);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpGet("api/stocks/{symbol}/history", Name = "GetStockHistory")]
        public IEnumerable<StockHistory> GetStockHistory(string symbol, DateTime? date, int pageNumber, int pageSize)
        {
            var query = _stockRepo.GetStockHistory(symbol).AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(h => h.TimeStamp.Date == date.Value.Date);
            }

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (pageNumber > totalPages)
            {
                pageNumber = totalPages > 0 ? totalPages : 1;
            }

            var skip = (pageNumber - 1) * pageSize;

            if (pageNumber > totalPages)
            {
                skip = totalCount - pageSize;
            }

            var results = query
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return results;
        }

        [HttpPost("api/stocks/{symbol}/buy/{quantity}", Name = "BuyStock")]
        public async Task<IActionResult> BuyStock(string symbol, int quantity, int price)
        {
            var stock = _stockRepo.GetStocks().FirstOrDefault(s => s.StockSymbol == symbol);

            if (stock == null)
            {
                return NotFound();
            }

            var boughtStock = new BoughtStock
            {
                StockId = stock.StockId,
                StockSymbol = symbol,
                Price = price,
                Quantity = quantity
            };

            _context.BoughtStocks.Add(boughtStock);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("api/stocks/boughtstocks", Name = "BoughtStocks")]
        public IEnumerable<BoughtStock> GetBoughtStocks()
        {
            return _context.BoughtStocks.AsNoTracking().ToList();
        }
    }
}
