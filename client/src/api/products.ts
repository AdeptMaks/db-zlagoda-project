import api from './client'
import type { Product } from '@/types'

export interface ProductPayload {
  categoryNumber: number
  productName: string
  characteristics: string
}

export const productsApi = {
  getAll: (params?: { search?: string; categoryNumber?: number }) =>
    api.get<Product[]>('/products', { params }).then((r) => r.data),
  getById: (id: number) => api.get<Product>(`/products/${id}`).then((r) => r.data),
  create: (body: ProductPayload) => api.post<Product>('/products', body).then((r) => r.data),
  update: (id: number, body: ProductPayload) =>
    api.put<Product>(`/products/${id}`, body).then((r) => r.data),
  remove: (id: number) => api.delete(`/products/${id}`).then((r) => r.data),
}
