using StockExchangeAPI.Models;
using Microsoft.EntityFrameworkCore;

public class InMemoryStockRepository : IStockRepository
{
    private readonly StockExchangeDBContext _context;

    public InMemoryStockRepository(StockExchangeDBContext context)
    {
        _context = context;
    }

    public IEnumerable<Stock> GetStocks()
    {
        return _context.Stocks.AsNoTracking().ToList();
    }

    public IEnumerable<StockHistory> GetStockHistory(string symbol)
    {
        return _context.StockHistories.AsNoTracking().Where(s => s.StockSymbol == symbol).ToList();
    }

    public void UpdateStocks(IEnumerable<Stock> stocks)
    {
        _context.Stocks.RemoveRange(_context.Stocks); // Clear existing stocks
        _context.Stocks.AddRange(stocks);
        _context.SaveChanges();
    }
}
