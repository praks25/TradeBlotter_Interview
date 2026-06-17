namespace TradeBlotter_Interview.DTOs;

public record TradeResponse(
    int Id,
    string Symbol,
    string Side,
    int Quantity,
    decimal Price,
    DateTime Timestamp
)
{
    public decimal NotionalValue => Quantity * Price;
}
