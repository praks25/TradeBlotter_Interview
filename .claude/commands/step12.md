Update frontend/src/App.vue to compose all three components into a complete layout.

Layout using CSS Grid:
  - Header bar at top: "Trade Blotter" title (dark background, white text, padding)
  - On wide screens (min-width: 1200px): 3-column grid
      Column 1 (~280px): TradeEntryForm
      Column 2 (1fr, fills remaining): BlotterTable
      Column 3 (~280px): PositionsPanel
  - On smaller screens: stack vertically in order — form, blotter, positions

On mounted():
  const store = useTradeStore()
  await Promise.all([store.fetchTrades(), store.fetchPositions()])

Add a full-page loading overlay (semi-transparent, centered spinner text "Loading...") shown when store.loading is true.

Remove all Vite/Vue boilerplate (HelloWorld component etc.) from App.vue and main.ts.
