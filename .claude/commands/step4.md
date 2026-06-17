Create IPositionService and PositionService in TradeBlotter_Interview/Services/.

PositionService.CalculatePositions(IEnumerable<Trade> trades) returns IEnumerable<Position>.

Algorithm (iterate trades ordered by Timestamp ascending):
- For each Buy:
    new_avg_cost = (current_qty * current_avg_cost + qty * price) / (current_qty + qty)
    new_qty = current_qty + qty
- For each Sell:
    new_qty = current_qty - qty
    avg_cost unchanged
- After processing all trades: exclude any symbol where NetQuantity == 0
- Return remaining positions

Also create ITradeService / TradeService in TradeBlotter_Interview/Services/ wrapping DbContext:
  Task<IEnumerable<Trade>> GetAllAsync()  — returns all trades, newest first (OrderByDescending Timestamp)
  Task<Trade> AddAsync(CreateTradeRequest request) — maps request to Trade, sets Timestamp = DateTime.UtcNow, saves to DB, returns saved entity

Register both services as Scoped in Program.cs.
