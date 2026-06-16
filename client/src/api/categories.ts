import api from './client'
import type { Category } from '@/types'

export const categoriesApi = {
  getAll: () => api.get<Category[]>('/categories').then((r) => r.data),
  create: (categoryName: string) =>
    api.post<Category>('/categories', { categoryName }).then((r) => r.data),
  update: (number: number, categoryName: string) =>
    api.put<Category>(`/categories/${number}`, { categoryName }).then((r) => r.data),
  remove: (number: number) => api.delete(`/categories/${number}`).then((r) => r.data),
}
