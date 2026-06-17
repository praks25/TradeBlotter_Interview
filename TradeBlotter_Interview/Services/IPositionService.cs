using TradeBlotter_Interview.Models;

namespace TradeBlotter_Interview.Services;

public interface IPositionService
{
    IEnumerable<Position> CalculatePositions(IEnumerable<Trade> trades);
}
