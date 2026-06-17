Create frontend/src/components/BlotterTable.vue using Vue 3 Composition API (<script setup lang="ts">).

Pull trades from useTradeStore().trades and display in a table.

Columns (in order): Timestamp | Symbol | Side | Qty | Price | Notional Value

Requirements:
  - Timestamp: format as "Jun 16 2026 14:32:05" using toLocaleDateString + toLocaleTimeString
  - Price: right-aligned, monospace, formatted as $0.00
  - Notional Value: right-aligned, monospace, formatted as $1,234.56 (with comma separator)
  - Side cell: render a pill/badge — "Buy" with green background (#dcfce7, text #166534), "Sell" with red (#fee2e2, text #991b1b)
  - Column headers for Timestamp and Symbol are clickable — toggle sort asc/desc, show ▲ or ▼ indicator
  - Default sort: newest first (Timestamp desc)
  - Zebra striping on rows (alternate background)
  - Empty state row: "No trades yet. Submit your first trade above." (colspan=6, centered)

Sorting: use a local ref sortState: { column: string, dir: 'asc'|'desc' }. Compute sortedTrades as a computed ref. Never mutate store state.
