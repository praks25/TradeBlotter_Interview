In TradeBlotter_Interview.Tests/PositionServiceTests.cs, write xUnit tests for PositionService.CalculatePositions() covering:

1. Single buy: NetQuantity and AverageCost are correct
2. Two buys of same symbol: AverageCost is the weighted average (not simple average)
3. Buy then partial sell: NetQuantity reduced, AverageCost unchanged
4. Buy then full sell (qty matches exactly): symbol is excluded from results
5. Two different symbols: each has independent position, no cross-contamination
6. Empty trade list: returns empty collection
7. Mixed scenario: two symbols, multiple trades each — verify both positions are correct

For decimal comparisons use Assert.Equal(expected, actual, precision: 4).
PositionService takes IEnumerable<Trade> directly — no database needed in these tests.
Create Trade objects inline with object initializers.
