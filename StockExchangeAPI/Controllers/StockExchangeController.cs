using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockExchangeAPI.Models;

namespace StockExchangeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StockExchangeController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;

        public StockExchangeController(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        [HttpGet("api/stocks", Name = "GetStocks")]
        public IEnumerable<Stock> Get()
        {
            return _stockRepo.GetStocks();
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
    }
}
