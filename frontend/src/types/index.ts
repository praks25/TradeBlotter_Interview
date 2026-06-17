export type Side = 'Buy' | 'Sell'

export interface Trade {
  id: number
  symbol: string
  side: Side
  quantity: number
  price: number
  timestamp: string
  notionalValue: number
}

export interface Position {
  symbol: string
  netQuantity: number
  averageCost: number
  marketValue: number
}

export interface CreateTradeRequest {
  symbol: string
  side: Side
  quantity: number
  price: number
}
