using Microsoft.AspNetCore.Mvc;
using StockExchangeAPI.Models;

namespace StockExchangeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class StockExchangeController : ControllerBase
{
    private readonly IStockRepository _stockService;

    public StockExchangeController(IStockRepository stockService)
    {
        _stockService = stockService;
    }

    [HttpGet("api/stocks", Name = "GetStocks")]
    public IEnumerable<Stock> Get()
    {
        return _stockService.GetStocks();
    }

    [HttpGet("api/stocks/{symbol}/history", Name = "GetStockHistory")]
    public IEnumerable<StockExchange> GetStockHistory(string symbol)
    {
        // Replace this with your actual logic to retrieve historical data
        return Enumerable.Range(1, 10).Select(index => new StockExchange
        {
            StockSymbol = symbol,
            Price = Random.Shared.Next(100, 200),
            TimeStamp = DateTime.Now.AddMinutes(-index * 5)
        })
        .ToArray();
    }
}
