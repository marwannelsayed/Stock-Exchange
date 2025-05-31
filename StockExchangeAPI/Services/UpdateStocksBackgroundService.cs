using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StockExchangeAPI.Models;

public class UpdateStocksBackgroundService : BackgroundService
{
    private Dictionary<int, Timer> _stockTimers; // Dictionary to store timers for each stock
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UpdateStocksBackgroundService> _logger;
    private readonly IHubContext<StockHub> _hubContext;

    public UpdateStocksBackgroundService(IServiceScopeFactory scopeFactory, ILogger<UpdateStocksBackgroundService> logger, IHubContext<StockHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _hubContext = hubContext;
        _stockTimers = new Dictionary<int, Timer>();
    }

 protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<StockExchangeDBContext>();
                var stocks = context.Stocks.ToList();

                var randomList = stocks.OrderBy(x => Guid.NewGuid()).Take(Random.Shared.Next(1, stocks.Count + 1)).ToList();

                foreach (var stock in randomList)
                {
                    UpdateStockPrice(stock);
                }

                // Notify clients about the updated stock
                await _hubContext.Clients.All.SendAsync("ReceiveStockUpdate", randomList);
            }
            await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(2,5)), cancellationToken);
        }

    }

    private async void UpdateStockPrice(Stock stock)
    {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<StockExchangeDBContext>();

                    // Retrieve the stock from the database (in case it was modified externally)
                    var updatedStock = await context.Stocks.FindAsync(stock.StockId);

                    if (updatedStock != null)
                    {
                        try
                        {
                            // Simulate updating the stock price and timestamp (replace with actual update logic)
                            updatedStock.Price = Random.Shared.Next(100, 200);
                            updatedStock.TimeStamp = DateTime.Now;

                            stock.Price = updatedStock.Price;
                            // Create a new stock history entry
                            context.StockHistories.Add(new StockHistory
                            {
                                StockSymbol = updatedStock.StockSymbol,
                                TimeStamp = updatedStock.TimeStamp,
                                Price = updatedStock.Price
                            });

                            
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occurred while updating stock with ID {StockId}: {ErrorMessage}", updatedStock.StockId, ex.Message);
                        }

                        try
                        {
                            await context.SaveChangesAsync();
                        }
                        catch (DbUpdateException ex)
                        {
                            _logger.LogError(ex, "An error occurred while saving changes: {ErrorMessage}", ex.InnerException?.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating stock with ID {StockId}: {ErrorMessage}", stock.StockId, ex.Message);
            }
            finally
            {
            }
        
    }




   
}
