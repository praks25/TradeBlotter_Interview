# Trade Blotter — Project Story

## What This Application Is

A **Trade Blotter** is a tool every trading desk uses. It is the live, running log of every trade a trader has executed — a record of what was bought, what was sold, at what price, and when. From that history, the system derives the trader's current **positions**: how many shares of each stock they hold and what those shares cost on average.

This project implements a full-stack trade blotter as a web application. A user submits trades through a form, watches them appear instantly in a live table, and sees their open positions update in real time — all without ever refreshing the page.

---

## The Domain: Trades and Positions

### What a Trade Is

Every trade has five pieces of information:

| Field | Meaning |
|---|---|
| Symbol | The ticker of the stock (e.g. AAPL, MSFT) |
| Side | Whether it was a Buy or a Sell |
| Quantity | How many shares |
| Price | The price per share at execution |
| Timestamp | When the trade was recorded (always UTC, set server-side) |

The timestamp is always stamped by the server the moment the trade arrives — the client never provides it. This prevents backdating and keeps the audit trail trustworthy.

Symbols are normalized to uppercase on the server (`ToUpperInvariant`) so that "aapl" and "AAPL" are treated as the same security.

### What a Position Is — and Why It Is Never Stored

A **position** is the net result of all trades for a given symbol. It is not a piece of data that gets saved to the database. Instead, it is computed fresh on every request by walking the full trade history.

This is a deliberate architectural choice. If positions were stored separately, they could drift out of sync with the trades that produced them — a consistency problem that would require reconciliation logic, transaction locking, and eventual manual fixes. By deriving positions on demand from the trade history, consistency is guaranteed by design: the trades *are* the source of truth, and positions are simply a view over them.

### The Position Calculation — Weighted Average Cost

The most important business logic in the application is how **average cost** is calculated when a trader makes multiple buys at different prices.

**Naive approach (wrong):** Take the simple average of all buy prices.

**Correct approach:** Weighted average cost — each purchase's price is weighted by its share count.

**Example:**
- Buy 100 shares of AAPL @ $150 → cost basis: $15,000
- Buy 50 more shares of AAPL @ $200 → additional cost: $10,000
- Total: 150 shares, total cost $25,000
- Weighted average cost: $25,000 ÷ 150 = **$166.6667**

The formula applied on each Buy:
```
new_avg_cost = (existing_qty × existing_avg_cost + new_qty × new_price)
               ÷ (existing_qty + new_qty)
```

**On a Sell:** the quantity decreases, but the average cost of remaining shares does not change. Selling shares does not alter the cost basis of the shares you still hold.

**When net quantity reaches zero:** the symbol is excluded from the positions response entirely. A closed position is not an open position.

---

## Backend Architecture (C# / .NET 8)

### Layers

```
HTTP Request
     │
     ▼
Controllers        ← Validate input, map to/from DTOs, return HTTP responses
     │
     ▼
Services           ← Business logic and database access
     │
     ▼
DbContext          ← Entity Framework Core → SQLite
```

### The Models

**`Trade`** — the persisted entity. Stored in SQLite via Entity Framework Core. Simple properties, no business logic.

**`Position`** — a C# `record` that is never persisted. It is computed by `PositionService` and returned directly as a response. The `MarketValue` property is a computed expression (`NetQuantity × AverageCost`).

**`CreateTradeRequest`** — a DTO (Data Transfer Object) that represents what the frontend sends. It has validation attributes (`[Required]`, `[Range]`, `[RegularExpression]`) so that .NET's model binding rejects invalid input before it ever reaches service code.

**`TradeResponse`** — what the API returns for a trade. Includes `NotionalValue` (Quantity × Price) as a computed property, so the frontend never has to calculate it.

### Why Records?

`Position`, `CreateTradeRequest`, and `TradeResponse` are all C# `record` types. Records are immutable by default and have structural equality. For objects that represent a snapshot of data (rather than a tracked entity), records are the appropriate choice — they cannot be accidentally mutated after creation.

### Services

**`TradeService`** wraps the DbContext. It has exactly two responsibilities: fetch all trades (newest first), and save a new trade. It stamps the timestamp server-side and normalizes the symbol to uppercase.

**`PositionService`** is pure business logic. It takes `IEnumerable<Trade>` and returns `IEnumerable<Position>`. It has no database dependency — this is intentional. Pure functions are easy to test (no mocking needed) and easy to reason about.

Both services are registered as **Scoped** — they live for the duration of a single HTTP request and are disposed afterward.

### Database

SQLite was chosen for zero-configuration local development. The database file (`trades.db`) is created automatically on startup via `db.Database.EnsureCreated()`.

One non-obvious detail: the project's repository is stored inside a OneDrive-synced folder. SQLite's default WAL (Write-Ahead Logging) journal mode leaves persistent sidecar files (`.db-wal` and `.db-shm`) that cloud sync clients treat as independent files. If OneDrive syncs them out of order or lags on one, the database can silently become corrupt or appear to reset. The application explicitly switches to `journal_mode=DELETE` (rollback journal) on startup, which has no persistent sidecars and survives cloud-synced folders safely.

### CORS

The API allows requests from `http://localhost:5173` — the address Vite's development server uses. Without this, the browser would block all API calls from the frontend.

### Error Handling

Controllers catch exceptions and return `ProblemDetails`-formatted responses:
- **400 Bad Request** for validation failures (field-level errors via `ValidationProblem`)
- **500 Internal Server Error** for unexpected failures (database down, etc.)

All exceptions are also logged with structured logging so they appear in the console output with context.

---

## Frontend Architecture (Vue 3 / Pinia / Vite)

### State Management: The Pinia Store

The entire frontend application shares a single Pinia store (`useTradeStore`). This store holds:
- `trades` — the full list of all trades, newest first
- `positions` — the current open positions
- `loading` — whether any async operation is in progress
- `error` — the last error message, if any

**No component talks to the API directly.** All API calls go through store actions. This means both `BlotterTable` and `PositionsPanel` react to the same data source — when a trade is submitted, both update together because they both read from the same store state.

The three actions:
- `fetchTrades()` — loads all trades from `GET /api/trades`
- `fetchPositions()` — loads positions from `GET /api/positions`
- `submitTrade(req)` — posts to `POST /api/trades`, then refreshes both trades and positions in parallel (`Promise.all`) in a single loading cycle

`submitTrade` manages `loading` itself and calls the API functions directly (rather than delegating to `fetchTrades`/`fetchPositions`) so that the loading flag is toggled exactly once, cleanly, with a single `finally` block.

### The Vite Proxy

During development, the frontend runs on `http://localhost:5173` and the backend runs on `http://localhost:5000`. To avoid hardcoding the backend URL in every API call, Vite is configured to proxy any request starting with `/api` to the backend:

```
Frontend calls: /api/trades
Vite rewrites to: http://localhost:5000/trades
```

This means all API calls in the frontend code simply use `/api/trades` — no environment-specific URLs anywhere in the code.

### The Three Components

**`TradeEntryForm`**
- Symbol input auto-uppercases on every keystroke
- Side is a pair of styled toggle buttons (Buy = green, Sell = red), not a dropdown
- Validation runs client-side before the store is touched; errors appear inline under each field
- On successful submit, the form clears itself and emits `trade-submitted`

**`BlotterTable`**
- Renders all trades from the store, sorted newest first by default
- Timestamp formatted as human-readable local time (e.g. "Jun 17 2026 14:32:05")
- Side column shows a colored badge/pill — green for Buy, red for Sell — so the trader can scan the log at a glance without reading every word
- Price and Notional Value are right-aligned in monospace font — numbers in a financial table must line up on their decimal points
- Column headers for Timestamp and Symbol are clickable to sort ascending or descending; active column shows a ▲/▼ indicator
- Sorting is done in a local `computed` ref — the store's trade array is never mutated

**`PositionsPanel`**
- Shows each open position as a card
- Long positions (net quantity > 0) have a green left border
- Short positions (net quantity < 0, edge case) have a red left border
- Average cost is displayed to 4 decimal places with a `$` prefix (`$166.6667`) — more precision than notional value because small cost basis differences matter for P&L calculation
- When all positions are closed, displays "No open positions."

---

## The Data Flow: End to End

1. User fills in the form and clicks **Submit Trade**
2. `TradeEntryForm` validates the input client-side
3. `store.submitTrade(req)` is called — sets `loading = true`
4. `POST /api/trades` is sent to the backend
5. `TradesController` validates the request via ModelState
6. `TradeService.AddAsync` stamps the timestamp, normalizes the symbol, saves to SQLite
7. Backend returns `201 Created` with the saved trade as a `TradeResponse`
8. Frontend (still in `submitTrade`) now calls `Promise.all([getTrades(), getPositions()])`
9. `GET /api/trades` returns all trades newest first — store updates `this.trades`
10. `GET /api/positions` fetches all trades, runs `PositionService.CalculatePositions`, returns open positions — store updates `this.positions`
11. `loading = false` in the `finally` block
12. `BlotterTable` re-renders with the new trade at the top
13. `PositionsPanel` re-renders with the updated position

All of this happens without a page reload. The user sees the blotter update within the time it takes for the API round trips.

---

## Testing

Unit tests cover `PositionService.CalculatePositions` exclusively — this is the only piece of logic complex enough to warrant tests. The controllers and services are straightforward CRUD wrappers; the position calculation is where bugs can hide.

The test suite covers:
1. Single buy — correct quantity and average cost
2. Two buys of the same symbol — weighted average (not simple average)
3. Buy then partial sell — quantity reduced, average cost unchanged
4. Buy then full sell — symbol excluded from results (net qty = 0)
5. Two different symbols — fully independent positions, no cross-contamination
6. Empty trade list — returns empty collection
7. Mixed scenario — two symbols with multiple buys and sells each

Tests use `NullLogger<PositionService>.Instance` so no logging infrastructure is needed. `PositionService` takes `IEnumerable<Trade>` directly so no database or mock is needed. Tests are fast, isolated, and fully deterministic.

---

## What the Evaluators Are Looking For

The interview brief explicitly called out these criteria:

| Criterion | How This Project Addresses It |
|---|---|
| Domain modeling | `Position` is never stored — derived on demand. `Trade` is the source of truth. Records used for immutable types. |
| Position logic correctness | Weighted average cost on buys, unchanged on sells. Net-zero symbols excluded. 7 unit tests prove it. |
| API design | Clean endpoint contracts, `ProblemDetails` for errors, correct HTTP status codes (201, 400, 500). |
| Vue patterns | `<script setup>` Composition API throughout. Pinia as single source of truth. No Options API. |
| UI judgment | Color-coded Buy/Sell badges, right-aligned monospace numbers, sortable columns, empty states, inline validation. |
| Tests | Position calculation fully covered with edge cases. No mocking needed — pure function design. |
| Repo quality | Clear README, Claude transcript included, granular commits. |
