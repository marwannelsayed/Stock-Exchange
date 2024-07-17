using StockExchangeAPI.Models;
using Microsoft.EntityFrameworkCore;

public interface IStockRepository
{
    public IEnumerable<Stock> GetStocks();

    public IEnumerable<StockHistory> GetStockHistory(string symbol);

    public void UpdateStocks(IEnumerable<Stock> stocks);

}
