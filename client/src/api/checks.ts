import api from './client'
import type { CheckDetails, StoreCheck } from '@/types'

export interface CreateCheckPayload {
  cardNumber?: string | null
  items: { upc: string; quantity: number }[]
}

export interface CheckFilter {
  employeeId?: string
  from?: string
  to?: string
}

export const checksApi = {
  getAll: (params?: CheckFilter) =>
    api.get<StoreCheck[]>('/checks', { params }).then((r) => r.data),
  getById: (checkNumber: string) =>
    api.get<CheckDetails>(`/checks/${checkNumber}`).then((r) => r.data),
  create: (body: CreateCheckPayload) =>
    api.post<CheckDetails>('/checks', body).then((r) => r.data),
  remove: (checkNumber: string) => api.delete(`/checks/${checkNumber}`).then((r) => r.data),
  totalSum: (params?: CheckFilter) =>
    api.get<{ total: number }>('/checks/stats/sum', { params }).then((r) => r.data),
  productQuantity: (params: { productId: number; from?: string; to?: string }) =>
    api
      .get<{ quantity: number }>('/checks/stats/product-quantity', { params })
      .then((r) => r.data),
}
