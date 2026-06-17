using TradeBlotter_Interview.Models;
using TradeBlotter_Interview.Services;

namespace TradeBlotter_Interview.Tests;

public class PositionServiceTests
{
    private readonly PositionService _sut = new();

    private static Trade Buy(string symbol, int qty, decimal price, DateTime timestamp) =>
        new() { Symbol = symbol, Side = "Buy", Quantity = qty, Price = price, Timestamp = timestamp };

    private static Trade Sell(string symbol, int qty, decimal price, DateTime timestamp) =>
        new() { Symbol = symbol, Side = "Sell", Quantity = qty, Price = price, Timestamp = timestamp };

    // 1. Single buy
    [Fact]
    public void SingleBuy_ReturnsCorrectQuantityAndAverageCost()
    {
        var trades = new[] { Buy("AAPL", 100, 150m, T(1)) };

        var positions = _sut.CalculatePositions(trades).ToList();

        Assert.Single(positions);
        Assert.Equal(100, positions[0].NetQuantity);
        Assert.Equal(150m, positions[0].AverageCost, precision: 4);
    }

    // 2. Two buys of same symbol — weighted average, not simple average
    [Fact]
    public void TwoBuysSameSymbol_AverageCostIsWeightedAverage()
    {
        // (100*150 + 50*200) / 150 = 25000/150 = 166.6667
        var trades = new[]
        {
            Buy("AAPL", 100, 150m, T(1)),
            Buy("AAPL",  50, 200m, T(2)),
        };

        var positions = _sut.CalculatePositions(trades).ToList();

        Assert.Single(positions);
        Assert.Equal(150, positions[0].NetQuantity);
        Assert.Equal(166.6667m, positions[0].AverageCost, precision: 4);
    }

    // 3. Buy then partial sell — quantity reduced, average cost unchanged
    [Fact]
    public void BuyThenPartialSell_ReducesQuantityKeepsAverageCost()
    {
        var trades = new[]
        {
            Buy("AAPL",  100, 150m, T(1)),
            Sell("AAPL",  40, 155m, T(2)),
        };

        var positions = _sut.CalculatePositions(trades).ToList();

        Assert.Single(positions);
        Assert.Equal(60, positions[0].NetQuantity);
        Assert.Equal(150m, positions[0].AverageCost, precision: 4);
    }

    // 4. Buy then full sell — symbol excluded from results
    [Fact]
    public void BuyThenFullSell_SymbolExcludedFromResults()
    {
        var trades = new[]
        {
            Buy("AAPL",  100, 150m, T(1)),
            Sell("AAPL", 100, 160m, T(2)),
        };

        var positions = _sut.CalculatePositions(trades).ToList();

        Assert.Empty(positions);
    }

    // 5. Two different symbols — each position is independent
    [Fact]
    public void TwoDifferentSymbols_EachPositionIsIndependent()
    {
        var trades = new[]
        {
            Buy("AAPL", 100, 150m, T(1)),
            Buy("MSFT", 200, 300m, T(2)),
        };

        var positions = _sut.CalculatePositions(trades)
            .ToDictionary(p => p.Symbol, StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, positions.Count);

        Assert.Equal(100,   positions["AAPL"].NetQuantity);
        Assert.Equal(150m,  positions["AAPL"].AverageCost, precision: 4);

        Assert.Equal(200,   positions["MSFT"].NetQuantity);
        Assert.Equal(300m,  positions["MSFT"].AverageCost, precision: 4);
    }

    // 6. Empty trade list — returns empty collection
    [Fact]
    public void EmptyTradeList_ReturnsEmptyCollection()
    {
        var positions = _sut.CalculatePositions(Enumerable.Empty<Trade>()).ToList();

        Assert.Empty(positions);
    }

    // 7. Mixed scenario — two symbols, multiple trades each
    [Fact]
    public void MixedScenario_TwoSymbolsMultipleTradesEach_BothPositionsCorrect()
    {
        // AAPL: Buy 100@150, Buy 50@200, Sell 30
        //   after buy1:  qty=100, avg=150
        //   after buy2:  qty=150, avg=(100*150+50*200)/150 = 25000/150 = 166.6667
        //   after sell:  qty=120, avg=166.6667 (unchanged)
        //
        // MSFT: Buy 200@300, Sell 100
        //   after buy:   qty=200, avg=300
        //   after sell:  qty=100, avg=300 (unchanged)
        var trades = new[]
        {
            Buy("AAPL",  100, 150m, T(1)),
            Buy("MSFT",  200, 300m, T(2)),
            Buy("AAPL",   50, 200m, T(3)),
            Sell("MSFT", 100, 310m, T(4)),
            Sell("AAPL",  30, 170m, T(5)),
        };

        var positions = _sut.CalculatePositions(trades)
            .ToDictionary(p => p.Symbol, StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, positions.Count);

        Assert.Equal(120,      positions["AAPL"].NetQuantity);
        Assert.Equal(166.6667m, positions["AAPL"].AverageCost, precision: 4);

        Assert.Equal(100,   positions["MSFT"].NetQuantity);
        Assert.Equal(300m,  positions["MSFT"].AverageCost, precision: 4);
    }

    private static DateTime T(int second) => new DateTime(2024, 1, 1, 0, 0, second, DateTimeKind.Utc);
}
