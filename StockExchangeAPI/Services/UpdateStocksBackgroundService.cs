using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using StockExchangeAPI.Models;
using Microsoft.Extensions.Logging;

public class UpdateStocksBackgroundService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UpdateStocksBackgroundService> _logger;

    public UpdateStocksBackgroundService(IServiceScopeFactory scopeFactory, ILogger<UpdateStocksBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(UpdateStocks, null, TimeSpan.Zero, TimeSpan.FromSeconds(5)); // Adjust the interval as needed
        return Task.CompletedTask;
    }

    private void UpdateStocks(object state)
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<StockExchangeDBContext>();
                var stockService = scope.ServiceProvider.GetRequiredService<IStockRepository>();

                var newStocks = stockService.GenerateStocks();

                foreach (var newStock in newStocks)
                {
                    var existingStock = context.Stocks.FirstOrDefault(s => s.StockSymbol == newStock.StockSymbol);
                    if (existingStock != null)
                    {
                        existingStock.Price = newStock.Price;
                        existingStock.TimeStamp = newStock.TimeStamp;
                    }
                    else
                    {
                        context.Stocks.Add(newStock);
                    }
                }

                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating stocks.");
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
