using TradeBlotter_Interview.Models;

namespace TradeBlotter_Interview.Services;

public class PositionService : IPositionService
{
    public IEnumerable<Position> CalculatePositions(IEnumerable<Trade> trades)
    {
        var state = new Dictionary<string, (int Qty, decimal AvgCost)>(StringComparer.OrdinalIgnoreCase);

        foreach (var trade in trades.OrderBy(t => t.Timestamp))
        {
            state.TryGetValue(trade.Symbol, out var current);
            var (qty, avgCost) = current;

            if (trade.Side == "Buy")
            {
                var newQty = qty + trade.Quantity;
                var newAvgCost = (qty * avgCost + trade.Quantity * trade.Price) / newQty;
                state[trade.Symbol] = (newQty, newAvgCost);
            }
            else
            {
                state[trade.Symbol] = (qty - trade.Quantity, avgCost);
            }
        }

        return state
            .Where(kvp => kvp.Value.Qty != 0)
            .Select(kvp => new Position(kvp.Key, kvp.Value.Qty, kvp.Value.AvgCost));
    }
}
