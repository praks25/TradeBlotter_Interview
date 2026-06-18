<script setup lang="ts">
import { useTradeStore } from '@/stores/tradeStore'

const store = useTradeStore()

function formatAvgCost(value: number): string {
  return '$' + value.toFixed(4)
}

function formatMarketValue(value: number): string {
  return value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}
</script>

<template>
  <div class="positions-panel">
    <h2 class="panel-title">Positions</h2>

    <p v-if="store.positions.length === 0" class="empty-state">No open positions.</p>

    <div v-else class="position-cards">
      <div
        v-for="position in store.positions"
        :key="position.symbol"
        class="position-card"
        :class="position.netQuantity > 0 ? 'long' : 'short'"
      >
        <div class="symbol">{{ position.symbol }}</div>
        <div class="row">
          <span class="label">Net Qty</span>
          <span class="value">{{ position.netQuantity }}</span>
        </div>
        <div class="row">
          <span class="label">Avg Cost</span>
          <span class="value">{{ formatAvgCost(position.averageCost) }}</span>
        </div>
        <div class="row">
          <span class="label">Market Value</span>
          <span class="value">{{ formatMarketValue(position.marketValue) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.positions-panel {
  max-width: 280px;
  width: 100%;
}

.panel-title {
  font-size: 1rem;
  font-weight: 600;
  margin: 0 0 0.75rem;
}

.empty-state {
  color: #6b7280;
  font-size: 0.9rem;
}

.position-cards {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.position-card {
  border-left: 4px solid transparent;
  border-radius: 4px;
  padding: 0.6rem 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.position-card.long {
  border-left-color: #16a34a;
  background-color: #f0fdf4;
}

.position-card.short {
  border-left-color: #dc2626;
  background-color: #fef2f2;
}

.symbol {
  font-weight: 700;
  font-size: 1.1rem;
}

.row {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
}

.row .label {
  color: #6b7280;
}

.row .value {
  font-family: 'Courier New', Courier, monospace;
}
</style>
