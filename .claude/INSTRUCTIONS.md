# Trade Blotter — How to Run and What to Check

---

## Prerequisites

Before starting, confirm you have:

| Tool | Minimum Version | Check Command |
|---|---|---|
| .NET SDK | 8.0 | `dotnet --version` |
| Node.js | 18+ | `node --version` |
| npm | 9+ | `npm --version` |

If either `dotnet` or `node` is not found, see the README for install instructions.

---

## Starting the Application

You need **two terminals open at the same time** — one for the backend, one for the frontend. They run concurrently.

### Terminal 1 — Backend (API)

```
cd TradeBlotter_Interview\TradeBlotter_Interview
dotnet run
```

**Expected output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started.
```

The API is ready when you see "Application started."

A file called `trades.db` will be created automatically in the same folder the first time it runs. This is the SQLite database.

### Terminal 2 — Frontend (Vue)

```
cd TradeBlotter_Interview\frontend
npm install        ← only needed the first time, or after pulling new changes
npm run dev
```

**Expected output:**
```
  VITE v5.x.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
```

### Open the Application

With both terminals running, open your browser and go to:

```
http://localhost:5173
```

You should see the Trade Blotter UI with the entry form, empty blotter table, and empty positions panel.

---

## Running the Tests

In a separate terminal (backend can stay running):

```
cd TradeBlotter_Interview
dotnet test
```

**Expected output:**
```
Passed!  - Failed: 0, Passed: 7, Skipped: 0, Total: 7
```

All 7 tests must pass. If any fail, do not submit.

---

## Smoke Test Checklist

Work through these in order after both servers are running.

### 1. Validation — empty form

- Click **Submit Trade** without filling in anything
- **Expected:** Four inline error messages appear — one under each field (Symbol, Side, Quantity, Price)
- The form must not call the API when invalid

### 2. First trade — single buy

- Enter: Symbol = `AAPL`, Side = `Buy`, Quantity = `100`, Price = `150`
- Click **Submit Trade**
- **Expected:**
  - Button shows "Submitting..." briefly
  - Form clears automatically after success
  - Blotter table shows one row: AAPL | Buy (green badge) | 100 | 150.00 | 15,000.00
  - Positions panel shows one card: AAPL, Net Qty 100, Avg Cost $150.0000

### 3. Second buy — same symbol, verify weighted average

- Enter: Symbol = `AAPL`, Side = `Buy`, Quantity = `50`, Price = `200`
- Click **Submit Trade**
- **Expected:**
  - Blotter now shows 2 rows (newest first)
  - Positions panel shows AAPL: Net Qty **150**, Avg Cost **$166.6667**
  - Verify the math: (100×150 + 50×200) / 150 = 25000/150 = 166.6667 ✓

### 4. Partial sell — verify avg cost unchanged

- Enter: Symbol = `AAPL`, Side = `Sell`, Quantity = `50`, Price = `175`
- Click **Submit Trade**
- **Expected:**
  - Blotter shows 3 rows; sell row has red badge
  - Positions panel: AAPL Net Qty **100**, Avg Cost still **$166.6667** (unchanged by the sell)

### 5. Second symbol — verify independence

- Enter: Symbol = `MSFT`, Side = `Buy`, Quantity = `200`, Price = `300`
- Click **Submit Trade**
- **Expected:**
  - Positions panel now shows **two cards**: AAPL and MSFT
  - MSFT: Net Qty 200, Avg Cost $300.0000
  - AAPL position is unchanged

### 6. Full sell — verify position closes

- Enter: Symbol = `AAPL`, Side = `Sell`, Quantity = `100`, Price = `180`
- Click **Submit Trade**
- **Expected:**
  - Blotter shows the sell row
  - Positions panel now shows **only MSFT** — AAPL has disappeared (net qty = 0)

### 7. Blotter sorting

- Click the **Timestamp** column header
- **Expected:** Sort order reverses; ▼ becomes ▲ (or vice versa)
- Click the **Symbol** column header
- **Expected:** Rows sort alphabetically by symbol; indicator moves to Symbol column

### 8. Buy/Sell visual distinction

- Scan the blotter rows
- **Expected:** All Buy rows have a green pill badge, all Sell rows have a red pill badge — readable at a glance

---

## Verifying the API Directly (Optional)

With the backend running, the Swagger UI is available at:

```
http://localhost:5000/swagger
```

You can call the endpoints directly from there to test the API independently of the frontend:

- `POST /trades` — submit a trade body like `{ "symbol": "AAPL", "side": "Buy", "quantity": 100, "price": 150 }`
- `GET /trades` — should return all trades, newest first
- `GET /positions` — should return derived positions only for symbols with non-zero net quantity

### Testing validation via Swagger

Try submitting invalid data and confirm you get a `400` with field-level error details:
- Empty symbol → 400
- Side = "Hold" → 400 (only "Buy" and "Sell" are accepted)
- Quantity = 0 → 400
- Price = -5 → 400

---

## Common Issues

| Symptom | Likely Cause | Fix |
|---|---|---|
| `dotnet` not found | .NET 8 SDK not installed or not on PATH | Restart terminal after install; check with `dotnet --version` |
| `npm` not found | Node.js installed but terminal opened before install | Close and reopen the terminal |
| Blank page at localhost:5173 | Frontend started but backend not running | Start the backend in a second terminal first |
| API calls fail in the browser (network errors) | Backend not running, or running on a different port | Confirm backend shows "Now listening on: http://localhost:5000" |
| `trades.db` appears reset / empty | OneDrive sync interfered with SQLite WAL files | Already handled — the app uses rollback journal mode; restart the backend |
| CORS error in browser console | Frontend making requests to a different port than 5173 | Ensure `npm run dev` is running (not a built version) |
| Tests fail with "type not found" | Test project reference not set up correctly | Run `dotnet build` from the solution root first |

---

## Project Structure Quick Reference

```
TradeBlotter_Interview/
├── TradeBlotter_Interview/          ← .NET 8 Web API
│   ├── Controllers/                 ← TradesController, PositionsController
│   ├── DTOs/                        ← CreateTradeRequest, TradeResponse
│   ├── Models/                      ← Trade, Position
│   ├── Services/                    ← TradeService, PositionService + interfaces
│   ├── Data/                        ← TradeBlotterDbContext (EF Core)
│   └── Program.cs                   ← DI registration, middleware, startup
├── TradeBlotter_Interview.Tests/    ← xUnit tests
│   └── PositionServiceTests.cs      ← 7 unit tests for position calculation
├── frontend/                        ← Vue 3 + Pinia + Vite + TypeScript
│   └── src/
│       ├── api/tradeApi.ts          ← Axios calls
│       ├── stores/tradeStore.ts     ← Pinia store (single source of truth)
│       ├── types/index.ts           ← TypeScript interfaces
│       └── components/
│           ├── TradeEntryForm.vue
│           ├── BlotterTable.vue
│           └── PositionsPanel.vue
├── .claude/
│   ├── commands/                    ← /step1 through /step14 slash commands
│   ├── STORY.md                     ← Full project narrative and design rationale
│   └── INSTRUCTIONS.md             ← This file
├── claude-transcript/               ← Full AI-assisted session log (required for submission)
└── README.md                        ← Setup instructions for evaluators
```

---

## Before Submitting

- [ ] `dotnet build` — zero warnings
- [ ] `dotnet test` — 7/7 passing
- [ ] `npm run build` (in frontend/) — production build succeeds
- [ ] All 8 smoke test scenarios above pass manually
- [ ] `claude-transcript/` folder contains the full Claude session log
- [ ] README has correct setup instructions
- [ ] Repo is pushed to a public GitHub repository
