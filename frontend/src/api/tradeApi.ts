import axios from 'axios'
import type { CreateTradeRequest, Position, Trade } from '@/types'

const http = axios.create({ baseURL: '/api' })

export async function getTrades(): Promise<Trade[]> {
  const { data } = await http.get<Trade[]>('/trades')
  return data
}

export async function postTrade(request: CreateTradeRequest): Promise<Trade> {
  const { data } = await http.post<Trade>('/trades', request)
  return data
}

export async function getPositions(): Promise<Position[]> {
  const { data } = await http.get<Position[]>('/positions')
  return data
}
