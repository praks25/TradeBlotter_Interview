Create frontend/src/components/TradeEntryForm.vue using Vue 3 Composition API (<script setup lang="ts">).

Form fields:
  - Symbol: text input, auto-uppercase on @input, placeholder "AAPL", max 10 chars
  - Side: two styled buttons acting as a toggle — "Buy" (green when selected) and "Sell" (red when selected). NOT a <select>.
  - Quantity: number input, min=1, integer only
  - Price: number input, min=0.01, step=0.01, show "$" prefix

Client-side validation with inline error messages (show under each field):
  - Symbol: required, non-empty after trim
  - Side: must be selected
  - Quantity: required, must be integer > 0
  - Price: required, must be > 0

On submit:
  - Validate all fields first — show errors if invalid, do not call store
  - If valid: set button to "Submitting..." (disabled), call useTradeStore().submitTrade()
  - On success: clear all form fields and errors
  - On error: show store.error below the submit button

Emit event 'trade-submitted' on success.

Style: Buy button green (#16a34a), Sell button red (#dc2626) when active. Use scoped <style>.
