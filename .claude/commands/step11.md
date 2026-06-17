Create frontend/src/components/PositionsPanel.vue using Vue 3 Composition API (<script setup lang="ts">).

Pull positions from useTradeStore().positions and display as a list of cards or a compact table.

For each position display:
  - Symbol — bold, 1.1rem
  - Net Qty — integer
  - Avg Cost — formatted as $0.0000 (4 decimal places for precision)
  - Market Value — formatted as $1,234.56

Visual treatment per card/row:
  - NetQuantity > 0: green left border (4px solid #16a34a), light green background
  - NetQuantity < 0: red left border (4px solid #dc2626), light red background (short position edge case)

Empty state: show "No open positions." in muted text when positions array is empty.

Keep the component narrow enough to sit in a sidebar (max-width ~280px on wide screens).
