Create the Vue 3 frontend in a /frontend folder at the repo root.

Run from the repo root:
  npm create vue@latest frontend -- --typescript --router false --pinia --vitest --eslint-with-prettier
  cd frontend && npm install axios

In frontend/vite.config.ts, add a server proxy so the frontend calls /api/* and Vite forwards to the backend:
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  }

Create frontend/src/types/index.ts with these TypeScript interfaces:
  export type Side = 'Buy' | 'Sell'
  export interface Trade { id: number; symbol: string; side: Side; quantity: number; price: number; timestamp: string; notionalValue: number }
  export interface Position { symbol: string; netQuantity: number; averageCost: number; marketValue: number }
  export interface CreateTradeRequest { symbol: string; side: Side; quantity: number; price: number }

Create frontend/src/api/tradeApi.ts with:
  getTrades(): Promise<Trade[]>       → GET /api/trades
  postTrade(req): Promise<Trade>      → POST /api/trades
  getPositions(): Promise<Position[]> → GET /api/positions

All using axios. Export all three functions.
