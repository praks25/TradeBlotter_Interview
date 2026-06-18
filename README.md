# Trade Blotter

A full-stack trade entry and position tracking application built with .NET 8 and Vue 3.

## Tech Stack
- **Backend**: C# / .NET 8 Web API, Entity Framework Core, SQLite
- **Frontend**: Vue 3 (Composition API), Pinia, Vite, TypeScript, Axios

## Prerequisites
- .NET 8 SDK
- Node.js 20+

## Running Locally

### Backend
```
cd TradeBlotter_Interview
dotnet run
# API available at http://localhost:5000
```

### Frontend
```
cd frontend
npm install
npm run dev
# App available at http://localhost:5173
```

## Running Tests
```
dotnet test TradeBlotter_Interview.Tests
```

## API Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /trades | Submit a new trade |
| GET | /trades | All trades, newest first |
| GET | /positions | Derived net positions per symbol |

## Design Decisions
- Positions are derived at query time from trade history — never stored separately. This guarantees consistency.
- Weighted average cost: Buy trades update avg cost; Sell trades reduce quantity only, cost basis unchanged.
- Single Pinia store is the sole source of truth — BlotterTable and PositionsPanel both react to the same state.
- Vite proxy forwards /api/* to the backend, keeping frontend calls environment-agnostic.
- SQLite chosen for zero-config local setup; swap the connection string for SQL Server with no code changes.

## What I'd Improve With More Time

**Domain model & position analytics**
- Short position support: flip the sign convention and show unrealised P&L alongside net quantity so a Short is immediately distinguishable from a flat position at risk
- FIFO vs weighted-average cost toggle: many desks use FIFO for tax lot matching; the current WAC algorithm is correct for most equity desks but should be configurable
- Realised P&L on close: when a Sell reduces a Long to zero (or vice versa), record the gain/loss against the original cost basis
- Fixed Income accrued interest: coupon × (days since last payment / days in period) should be added to the position's market value for FI instruments
- FX base currency normalisation: FX trades need a notional in a single reporting currency; today the notional is face × price, which is meaningless across currency pairs

**Infrastructure & ops**
- WebSocket or SSE for real-time multi-tab synchronisation — today you must refresh to see another user's trades
- Docker Compose for single-command startup (API + SQLite volume mount)
- Swap SQLite for SQL Server or PostgreSQL for multi-user concurrency; the connection string is the only change needed
- Full integration test suite at the HTTP layer (WebApplicationFactory)

**UI / UX**
- Virtual scrolling on the blotter for large trade volumes (10k+ rows)
- Date-range picker and symbol typeahead on the blotter filter bar
- Sortable positions panel with one-click drill-down into the underlying trades for that symbol
- Export to CSV / Excel for downstream reconciliation

## Claude Transcript
See the `/claude-transcript/` folder for the complete AI-assisted development session log.
