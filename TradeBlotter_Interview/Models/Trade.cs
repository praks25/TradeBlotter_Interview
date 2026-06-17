namespace TradeBlotter_Interview.Models;

public class Trade
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Side { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }
}
