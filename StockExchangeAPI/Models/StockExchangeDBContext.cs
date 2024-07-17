using Microsoft.EntityFrameworkCore;

namespace StockExchangeAPI.Models
{
    public class StockExchangeDBContext : DbContext
    {
        public StockExchangeDBContext(DbContextOptions<StockExchangeDBContext> options)
            : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockHistory> StockHistories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BoughtStock> BoughtStocks { get; set; }
    }
}