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
- WebSocket or SSE for real-time multi-tab synchronization
- Virtual scrolling on the blotter for large trade volumes
- Pagination or date-range filtering on GET /trades
- Docker Compose for single-command startup
- Full integration test suite at the HTTP layer

## Claude Transcript
See the `/claude-transcript/` folder for the complete AI-assisted development session log.
