using Microsoft.EntityFrameworkCore;
using TradeBlotter_Interview.Data;
using TradeBlotter_Interview.DTOs;
using TradeBlotter_Interview.Models;

namespace TradeBlotter_Interview.Services;

public class TradeService(TradeBlotterDbContext db) : ITradeService
{
    public async Task<IEnumerable<Trade>> GetAllAsync() =>
        await db.Trades.OrderByDescending(t => t.Timestamp).ToListAsync();

    public async Task<Trade> AddAsync(CreateTradeRequest request)
    {
        var trade = new Trade
        {
            Symbol    = request.Symbol.ToUpperInvariant(),
            Side      = request.Side,
            Quantity  = request.Quantity,
            Price     = request.Price,
            Timestamp = DateTime.UtcNow,
        };

        db.Trades.Add(trade);
        await db.SaveChangesAsync();
        return trade;
    }
}
