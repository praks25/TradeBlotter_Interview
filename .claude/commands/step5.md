Create two controllers in TradeBlotter_Interview/Controllers/:

TradesController.cs:
- POST /trades
  - Accept [FromBody] CreateTradeRequest, check ModelState
  - Timestamp must be set server-side (DateTime.UtcNow) — ignore any client-provided value
  - Return 201 Created with TradeResponse body (include NotionalValue = Quantity * Price)
  - Return 400 with validation problem details if ModelState invalid
- GET /trades
  - Return 200 with List<TradeResponse>, newest first
  - Include NotionalValue in each item

PositionsController.cs:
- GET /positions
  - Fetch all trades via ITradeService
  - Pass to IPositionService.CalculatePositions()
  - Return 200 with the resulting position list
  - Symbols with NetQuantity == 0 are already excluded by the service

Use constructor injection for both services. Use ProducesResponseType attributes. Return ProblemDetails on errors.
