using Microsoft.AspNetCore.SignalR;
using StockExchangeAPI.Models;

public class StockHub : Hub
{
    public async Task SendStockUpdate(Stock stock)
    {
        await Clients.All.SendAsync("ReceiveStockUpdate", stock);
    }
}