using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StockExchangeAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class UpdateStocksBackgroundService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UpdateStocksBackgroundService> _logger;
    private readonly IHubContext<StockHub> _hubContext;

    public UpdateStocksBackgroundService(IServiceScopeFactory scopeFactory, ILogger<UpdateStocksBackgroundService> logger, IHubContext<StockHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _hubContext = hubContext;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(UpdateStocks, null, TimeSpan.Zero, TimeSpan.FromSeconds(5)); // Adjust the interval as needed
        return Task.CompletedTask;
    }

private async void UpdateStocks(object state)
{
    try
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<StockExchangeDBContext>();

            var stocksToUpdate = context.Stocks.ToList(); // Get all stocks from the database

            foreach (var stock in stocksToUpdate)
            {
                try
                {
                    // Simulate updating the stock price and timestamp (replace with actual update logic)
                    stock.Price = Random.Shared.Next(100, 200);
                    stock.TimeStamp = DateTime.Now;

                    // Create a new stock history entry
                    context.StockHistories.Add(new StockHistory
                    {
                        StockSymbol = stock.StockSymbol, // Ensure the StockSymbol is set
                        TimeStamp = stock.TimeStamp,
                        Price = stock.Price
                    });

                    // Notify clients about the updated stock
                    await _hubContext.Clients.All.SendAsync("ReceiveStockUpdate", stock);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating stock with ID {StockId}: {ErrorMessage}", stock.StockId, ex.Message);
                }
            }

try
{
    await context.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    // Log or print the inner exception message
    _logger.LogError(ex, "An error occurred while saving changes: {ErrorMessage}", ex.InnerException?.Message);
}
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while updating stocks: {ErrorMessage}", ex.Message);
    }
}


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
