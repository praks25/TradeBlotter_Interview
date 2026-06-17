using System.ComponentModel.DataAnnotations;

namespace TradeBlotter_Interview.DTOs;

public record CreateTradeRequest(
    [Required, MinLength(1)] string Symbol,
    [Required, RegularExpression("^(Buy|Sell)$", ErrorMessage = "Side must be 'Buy' or 'Sell'")] string Side,
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")] int Quantity,
    [Range(0.0001, double.MaxValue, ErrorMessage = "Price must be greater than 0")] decimal Price
);
