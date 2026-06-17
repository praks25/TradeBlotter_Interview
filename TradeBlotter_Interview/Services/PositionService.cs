using TradeBlotter_Interview.Models;

namespace TradeBlotter_Interview.Services;

public class PositionService(ILogger<PositionService> logger) : IPositionService
{
    // Walks trades oldest-to-newest, accumulating net quantity and weighted average
    // cost per symbol. Buys move the average cost; sells only reduce quantity.
    // Symbols that net out to zero are dropped from the result.
    public IEnumerable<Position> CalculatePositions(IEnumerable<Trade> trades)
    {
        try
        {
            var state = new Dictionary<string, (int Qty, decimal AvgCost)>(StringComparer.OrdinalIgnoreCase);

            foreach (var trade in trades.OrderBy(t => t.Timestamp))
            {
                state.TryGetValue(trade.Symbol, out var current);
                var (qty, avgCost) = current;

                if (trade.Side == "Buy")
                {
                    // Weighted average: blend the existing cost basis with the new fill.
                    var newQty = qty + trade.Quantity;
                    var newAvgCost = (qty * avgCost + trade.Quantity * trade.Price) / newQty;
                    state[trade.Symbol] = (newQty, newAvgCost);
                }
                else
                {
                    // Selling never changes the cost basis of the remaining shares.
                    state[trade.Symbol] = (qty - trade.Quantity, avgCost);
                }
            }

            var positions = state
                .Where(kvp => kvp.Value.Qty != 0)
                .Select(kvp => new Position(kvp.Key, kvp.Value.Qty, kvp.Value.AvgCost))
                .ToList();

            logger.LogInformation("Calculated {Count} open positions", positions.Count);

            return positions;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to calculate positions");
            throw;
        }
    }
}
