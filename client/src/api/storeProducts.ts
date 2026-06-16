import api from './client'
import type { StoreProduct, StoreProductDetails } from '@/types'

export interface StoreProductPayload {
  upc: string
  upcProm?: string | null
  productId: number
  sellingPrice: number
  productsNumber: number
  promotionalProduct: boolean
}

export const storeProductsApi = {
  getAll: (params?: { sort?: string; promotional?: boolean }) =>
    api.get<StoreProductDetails[]>('/store-products', { params }).then((r) => r.data),
  getByUpc: (upc: string) =>
    api.get<StoreProductDetails>(`/store-products/${upc}`).then((r) => r.data),
  create: (body: StoreProductPayload) =>
    api.post<StoreProduct>('/store-products', body).then((r) => r.data),
  update: (upc: string, body: StoreProductPayload) =>
    api.put<StoreProduct>(`/store-products/${upc}`, body).then((r) => r.data),
  remove: (upc: string) => api.delete(`/store-products/${upc}`).then((r) => r.data),
}
