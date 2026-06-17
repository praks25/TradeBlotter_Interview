using Microsoft.EntityFrameworkCore;
using TradeBlotter_Interview.Data;
using TradeBlotter_Interview.DTOs;
using TradeBlotter_Interview.Models;

namespace TradeBlotter_Interview.Services;

public class TradeService(TradeBlotterDbContext db, ILogger<TradeService> logger) : ITradeService
{
    // Returns every trade, newest first. Read-only, so failures are unexpected
    // (e.g. db unreachable) — log and let the controller decide how to respond.
    public async Task<IEnumerable<Trade>> GetAllAsync()
    {
        try
        {
            var trades = await db.Trades.OrderByDescending(t => t.Timestamp).ToListAsync();
            logger.LogInformation("Fetched {Count} trades", trades.Count);
            return trades;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch trades");
            throw;
        }
    }

    // Persists a new trade. Timestamp is always set server-side (UtcNow) so the
    // client can never backdate or forge a trade time. Symbol is normalized to
    // uppercase so "aapl" and "AAPL" are treated as the same position.
    public async Task<Trade> AddAsync(CreateTradeRequest request)
    {
        try
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

            logger.LogInformation(
                "Created trade {Id}: {Side} {Quantity} {Symbol} @ {Price}",
                trade.Id, trade.Side, trade.Quantity, trade.Symbol, trade.Price);

            return trade;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create trade for {Symbol}", request.Symbol);
            throw;
        }
    }
}
