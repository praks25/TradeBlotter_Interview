Create frontend/src/stores/tradeStore.ts as a Pinia store (defineStore, id: 'trades').

State:
  trades: Trade[]        — all trades, newest first
  positions: Position[]  — current open positions
  loading: boolean
  error: string | null

Actions:
  async fetchTrades()                          — calls getTrades(), sets this.trades
  async fetchPositions()                       — calls getPositions(), sets this.positions
  async submitTrade(req: CreateTradeRequest)   — calls postTrade(req), then awaits both fetchTrades() and fetchPositions() in parallel (Promise.all)

Set loading = true at start and false in a finally block for each action.
Set error on catch.

The store is the single source of truth. No component calls the API directly.
