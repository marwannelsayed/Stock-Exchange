using StockExchangeAPI.Models;
using Microsoft.EntityFrameworkCore;

public interface IStockRepository
{
    public IEnumerable<Stock> GetStocks();

    public IEnumerable<StockHistory> GetStockHistory(string symbol);

    public void UpdateStocks(IEnumerable<Stock> stocks);

    public IEnumerable<Stock> GenerateStocks()
    {
        return new List<Stock>
        {
            new Stock { StockSymbol = "AAPL", Price = Random.Shared.Next(100, 200), TimeStamp = DateTime.Now },
            new Stock { StockSymbol = "GOOGL", Price = Random.Shared.Next(100, 200), TimeStamp = DateTime.Now },
            new Stock { StockSymbol = "MSFT", Price = Random.Shared.Next(100, 200), TimeStamp = DateTime.Now },
            new Stock { StockSymbol = "AMZN", Price = Random.Shared.Next(100, 200), TimeStamp = DateTime.Now },
            new Stock { StockSymbol = "TSLA", Price = Random.Shared.Next(100, 200), TimeStamp = DateTime.Now }
        };
    }
}
