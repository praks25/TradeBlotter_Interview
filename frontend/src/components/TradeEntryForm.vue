<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useTradeStore } from '@/stores/tradeStore'
import type { Side } from '@/types'

const emit = defineEmits<{ (e: 'trade-submitted'): void }>()

const store = useTradeStore()

const symbol = ref('')
const side = ref<Side | null>(null)
const quantityInput = ref('')
const priceInput = ref('')
const submitting = ref(false)

const errors = reactive<{
  symbol?: string
  side?: string
  quantity?: string
  price?: string
}>({})

function onSymbolInput(e: Event) {
  symbol.value = (e.target as HTMLInputElement).value.toUpperCase()
}

function validate(): boolean {
  errors.symbol = undefined
  errors.side = undefined
  errors.quantity = undefined
  errors.price = undefined
  let valid = true

  if (!symbol.value.trim()) {
    errors.symbol = 'Symbol is required'
    valid = false
  }

  if (!side.value) {
    errors.side = 'Side must be selected'
    valid = false
  }

  const qty = Number(quantityInput.value)
  if (quantityInput.value.trim() === '' || !Number.isInteger(qty) || qty <= 0) {
    errors.quantity = 'Quantity must be a whole number greater than 0'
    valid = false
  }

  const prc = Number(priceInput.value)
  if (priceInput.value.trim() === '' || !(prc > 0)) {
    errors.price = 'Price must be greater than 0'
    valid = false
  }

  return valid
}

function resetForm() {
  symbol.value = ''
  side.value = null
  quantityInput.value = ''
  priceInput.value = ''
  errors.symbol = undefined
  errors.side = undefined
  errors.quantity = undefined
  errors.price = undefined
}

async function handleSubmit() {
  if (!validate()) return

  submitting.value = true
  await store.submitTrade({
    symbol: symbol.value.trim(),
    side: side.value as Side,
    quantity: Number(quantityInput.value),
    price: Number(priceInput.value),
  })
  submitting.value = false

  if (!store.error) {
    resetForm()
    emit('trade-submitted')
  }
}
</script>

<template>
  <form class="trade-entry-form" @submit.prevent="handleSubmit">
    <div class="field">
      <label for="symbol">Symbol</label>
      <input
        id="symbol"
        type="text"
        :value="symbol"
        placeholder="AAPL"
        maxlength="10"
        @input="onSymbolInput"
      />
      <span v-if="errors.symbol" class="error">{{ errors.symbol }}</span>
    </div>

    <div class="field">
      <label>Side</label>
      <div class="side-toggle">
        <button
          type="button"
          class="side-button buy"
          :class="{ active: side === 'Buy' }"
          @click="side = 'Buy'"
        >
          Buy
        </button>
        <button
          type="button"
          class="side-button sell"
          :class="{ active: side === 'Sell' }"
          @click="side = 'Sell'"
        >
          Sell
        </button>
      </div>
      <span v-if="errors.side" class="error">{{ errors.side }}</span>
    </div>

    <div class="field">
      <label for="quantity">Quantity</label>
      <input id="quantity" v-model="quantityInput" type="number" min="1" step="1" />
      <span v-if="errors.quantity" class="error">{{ errors.quantity }}</span>
    </div>

    <div class="field">
      <label for="price">Price</label>
      <div class="price-input">
        <span class="prefix">$</span>
        <input id="price" v-model="priceInput" type="number" min="0.01" step="0.01" />
      </div>
      <span v-if="errors.price" class="error">{{ errors.price }}</span>
    </div>

    <button type="submit" class="submit-button" :disabled="submitting">
      {{ submitting ? 'Submitting...' : 'Submit Trade' }}
    </button>

    <p v-if="store.error" class="submit-error">{{ store.error }}</p>
  </form>
</template>

<style scoped>
.trade-entry-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  max-width: 320px;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

label {
  font-weight: 600;
  font-size: 0.875rem;
}

input[type='text'],
input[type='number'] {
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  font-size: 1rem;
}

.side-toggle {
  display: flex;
  gap: 0.5rem;
}

.side-button {
  flex: 1;
  padding: 0.5rem;
  border: 2px solid #d1d5db;
  border-radius: 4px;
  background: #fff;
  cursor: pointer;
  font-weight: 600;
  transition:
    background-color 0.15s,
    border-color 0.15s,
    color 0.15s;
}

.side-button.buy.active {
  background-color: #16a34a;
  border-color: #16a34a;
  color: #fff;
}

.side-button.sell.active {
  background-color: #dc2626;
  border-color: #dc2626;
  color: #fff;
}

.price-input {
  display: flex;
  align-items: center;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  overflow: hidden;
}

.price-input .prefix {
  padding: 0 0.5rem;
  background: #f3f4f6;
  color: #374151;
}

.price-input input {
  border: none;
  flex: 1;
}

.error {
  color: #dc2626;
  font-size: 0.8rem;
}

.submit-button {
  padding: 0.6rem;
  background-color: #2563eb;
  color: #fff;
  border: none;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
}

.submit-button:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

.submit-error {
  color: #dc2626;
  font-size: 0.875rem;
  margin-top: 0.5rem;
}
</style>
