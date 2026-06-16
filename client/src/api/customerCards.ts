import api from './client'
import type { CustomerCard } from '@/types'

export interface CustomerCardPayload {
  surname: string
  firstname: string
  patronymic?: string | null
  phoneNumber: string
  city?: string | null
  street?: string | null
  zipCode?: string | null
  percent: number
}

export const customerCardsApi = {
  getAll: (params?: { search?: string; percent?: number }) =>
    api.get<CustomerCard[]>('/customer-cards', { params }).then((r) => r.data),
  create: (body: CustomerCardPayload) =>
    api.post<CustomerCard>('/customer-cards', body).then((r) => r.data),
  update: (cardNumber: string, body: CustomerCardPayload) =>
    api.put<CustomerCard>(`/customer-cards/${cardNumber}`, body).then((r) => r.data),
  remove: (cardNumber: string) => api.delete(`/customer-cards/${cardNumber}`).then((r) => r.data),
}
