<script setup lang="ts">
import { onMounted } from 'vue'
import { useTradeStore } from '@/stores/tradeStore'
import TradeEntryForm from '@/components/TradeEntryForm.vue'
import BlotterTable from '@/components/BlotterTable.vue'
import PositionsPanel from '@/components/PositionsPanel.vue'

const store = useTradeStore()

onMounted(async () => {
  await Promise.all([store.fetchTrades(), store.fetchPositions()])
})
</script>

<template>
  <div class="app">
    <header class="app-header">
      <h1>Trade Blotter</h1>
    </header>

    <div class="app-body">
      <section class="panel-entry">
        <TradeEntryForm />
      </section>

      <section class="panel-blotter">
        <BlotterTable />
      </section>

      <section class="panel-positions">
        <PositionsPanel />
      </section>
    </div>

    <div v-if="store.loading" class="loading-overlay">
      <span class="spinner-text">Loading...</span>
    </div>
  </div>
</template>

<style scoped>
.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-header {
  background-color: #111827;
  color: #ffffff;
  padding: 1rem 1.5rem;
}

.app-header h1 {
  margin: 0;
  font-size: 1.4rem;
}

.app-body {
  flex: 1;
  display: grid;
  grid-template-columns: 1fr;
  gap: 1.5rem;
  padding: 1.5rem;
}

@media (min-width: 1200px) {
  .app-body {
    grid-template-columns: 280px 1fr 280px;
    align-items: start;
  }
}

.loading-overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.spinner-text {
  background-color: #ffffff;
  padding: 0.75rem 1.5rem;
  border-radius: 6px;
  font-weight: 600;
  color: #111827;
}
</style>
