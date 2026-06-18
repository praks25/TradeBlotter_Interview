import { defineStore } from 'pinia'
import { getPositions, getTrades, postTrade } from '@/api/tradeApi'
import type { CreateTradeRequest, Position, Trade } from '@/types'

export const useTradeStore = defineStore('trades', {
  state: () => ({
    trades: [] as Trade[],
    positions: [] as Position[],
    loading: false,
    error: null as string | null,
  }),

  actions: {
    async fetchTrades() {
      this.loading = true
      this.error = null
      try {
        this.trades = await getTrades()
      } catch (e) {
        this.error = e instanceof Error ? e.message : 'Failed to fetch trades'
      } finally {
        this.loading = false
      }
    },

    async fetchPositions() {
      this.loading = true
      this.error = null
      try {
        this.positions = await getPositions()
      } catch (e) {
        this.error = e instanceof Error ? e.message : 'Failed to fetch positions'
      } finally {
        this.loading = false
      }
    },

    async submitTrade(req: CreateTradeRequest) {
      // submitTrade owns its own loading lifecycle. We call the API functions
      // directly (not the other actions) so loading isn't toggled multiple times.
      this.loading = true
      this.error = null
      try {
        await postTrade(req)
        const [trades, positions] = await Promise.all([getTrades(), getPositions()])
        this.trades = trades
        this.positions = positions
      } catch (e) {
        this.error = e instanceof Error ? e.message : 'Failed to submit trade'
      } finally {
        this.loading = false
      }
    },
  },
})
