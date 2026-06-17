namespace TradeBlotter_Interview.Models;

public record Position(string Symbol, int NetQuantity, decimal AverageCost)
{
    public decimal MarketValue => NetQuantity * AverageCost;
}
