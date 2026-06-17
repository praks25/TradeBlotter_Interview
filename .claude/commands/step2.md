In TradeBlotter_Interview/Models/, create two clean domain types:

Trade.cs:
- int Id
- string Symbol (e.g. "AAPL")
- string Side ("Buy" or "Sell")
- int Quantity
- decimal Price
- DateTime Timestamp (UTC, set server-side on creation)

Position.cs (not persisted — derived at runtime only):
- string Symbol
- int NetQuantity
- decimal AverageCost
- decimal MarketValue (NetQuantity * AverageCost, computed property)

In TradeBlotter_Interview/DTOs/, create:

CreateTradeRequest.cs — what the frontend POSTs:
- string Symbol (required, non-empty)
- string Side (required, must be "Buy" or "Sell")
- int Quantity (must be > 0)
- decimal Price (must be > 0)
Add System.ComponentModel.DataAnnotations validation attributes.

TradeResponse.cs — what the API returns per trade:
- All Trade fields plus NotionalValue (Quantity * Price, computed)

Use C# records where appropriate. Keep types immutable where possible.
