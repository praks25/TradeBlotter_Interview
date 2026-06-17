<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useTradeStore } from '@/stores/tradeStore'
import type { Trade } from '@/types'

type SortColumn = 'timestamp' | 'symbol'
type SortDir = 'asc' | 'desc'

const store = useTradeStore()

const sortState = reactive<{ column: SortColumn; dir: SortDir }>({
  column: 'timestamp',
  dir: 'desc',
})

function toggleSort(column: SortColumn) {
  if (sortState.column === column) {
    sortState.dir = sortState.dir === 'asc' ? 'desc' : 'asc'
  } else {
    sortState.column = column
    sortState.dir = 'asc'
  }
}

const sortedTrades = computed<Trade[]>(() => {
  const trades = [...store.trades]
  const { column, dir } = sortState

  trades.sort((a, b) => {
    const cmp =
      column === 'timestamp'
        ? new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
        : a.symbol.localeCompare(b.symbol)
    return dir === 'asc' ? cmp : -cmp
  })

  return trades
})

function formatTimestamp(ts: string): string {
  const date = new Date(ts)
  const datePart = date
    .toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
    .replace(',', '')
  const timePart = date.toLocaleTimeString('en-US', { hour12: false })
  return `${datePart} ${timePart}`
}

function formatPrice(price: number): string {
  return price.toFixed(2)
}

function formatNotional(value: number): string {
  return value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

function sortIndicator(column: SortColumn): string {
  if (sortState.column !== column) return ''
  return sortState.dir === 'asc' ? '▲' : '▼'
}
</script>

<template>
  <table class="blotter-table">
    <thead>
      <tr>
        <th class="sortable" @click="toggleSort('timestamp')">
          Timestamp <span class="sort-indicator">{{ sortIndicator('timestamp') }}</span>
        </th>
        <th class="sortable" @click="toggleSort('symbol')">
          Symbol <span class="sort-indicator">{{ sortIndicator('symbol') }}</span>
        </th>
        <th>Side</th>
        <th>Qty</th>
        <th>Price</th>
        <th>Notional Value</th>
      </tr>
    </thead>
    <tbody>
      <tr v-if="sortedTrades.length === 0">
        <td colspan="6" class="empty-state">No trades yet. Submit your first trade above.</td>
      </tr>
      <tr v-for="trade in sortedTrades" v-else :key="trade.id">
        <td>{{ formatTimestamp(trade.timestamp) }}</td>
        <td>{{ trade.symbol }}</td>
        <td>
          <span class="pill" :class="trade.side === 'Buy' ? 'buy' : 'sell'">{{ trade.side }}</span>
        </td>
        <td>{{ trade.quantity }}</td>
        <td class="numeric">{{ formatPrice(trade.price) }}</td>
        <td class="numeric">{{ formatNotional(trade.notionalValue) }}</td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped>
.blotter-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.blotter-table th,
.blotter-table td {
  padding: 0.5rem 0.75rem;
  text-align: left;
}

.blotter-table thead th {
  border-bottom: 2px solid #d1d5db;
  font-weight: 600;
  user-select: none;
}

th.sortable {
  cursor: pointer;
}

.sort-indicator {
  font-size: 0.7rem;
}

.blotter-table tbody tr:nth-child(odd) {
  background-color: #f9fafb;
}

.blotter-table tbody tr:nth-child(even) {
  background-color: #ffffff;
}

.numeric {
  text-align: right;
  font-family: 'Courier New', Courier, monospace;
}

.pill {
  display: inline-block;
  padding: 0.15rem 0.6rem;
  border-radius: 9999px;
  font-size: 0.8rem;
  font-weight: 600;
}

.pill.buy {
  background-color: #dcfce7;
  color: #166534;
}

.pill.sell {
  background-color: #fee2e2;
  color: #991b1b;
}

.empty-state {
  text-align: center;
  color: #6b7280;
  padding: 1.5rem 0.75rem;
}
</style>
