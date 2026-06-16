import api from './client'
import type { CategoryBuyer, CategoryRevenue, CashierPromo, ProductRevenue } from '@/types'

export interface ProductRevenueFilter {
  employeeId?: string
  from?: string
  to?: string
}

export interface DateRangeFilter {
  from?: string
  to?: string
}

export const statisticsApi = {
  productRevenue: (params?: ProductRevenueFilter) =>
    api.get<ProductRevenue[]>('/statistics/product-revenue', { params }).then((r) => r.data),
  categoryBuyers: (categoryNumber: number) =>
    api
      .get<CategoryBuyer[]>('/statistics/category-buyers', { params: { categoryNumber } })
      .then((r) => r.data),
  categoryRevenue: (params?: DateRangeFilter) =>
    api.get<CategoryRevenue[]>('/statistics/category-revenue', { params }).then((r) => r.data),
  cashiersSoldAllPromos: () =>
    api.get<CashierPromo[]>('/statistics/cashiers-sold-all-promos').then((r) => r.data),
}
