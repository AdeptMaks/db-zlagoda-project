export type EmployeeRole = 'Manager' | 'Cachier'

export interface Address {
  city: string
  street: string
  zipCode: string
}

export interface Employee {
  employeeId: string
  employeeRole: EmployeeRole
  surname: string
  firstname: string
  patronymic?: string | null
  salary: number
  startDate: string
  birthDate: string
  phoneNumber: string
  city: string
  street: string
  zipCode: string
}

export interface EmployeeContact {
  surname: string
  firstname: string
  patronymic?: string | null
  phoneNumber: string
  city: string
  street: string
  zipCode: string
}

export interface CreateEmployeeRequest {
  employeeRole: EmployeeRole
  surname: string
  firstname: string
  username: string
  password: string
  patronymic?: string | null
  salary: number
  startDate: string
  birthDate: string
  phoneNumber: string
  addressInfo?: Address | null
}

export interface UpdateEmployeeRequest {
  employeeRole: EmployeeRole
  surname: string
  firstname: string
  patronymic?: string | null
  salary: number
  startDate: string
  birthDate: string
  phoneNumber: string
  addressInfo?: Address | null
}

export interface LoginRequest {
  username: string
  password: string
}

export interface Category {
  categoryNumber: number
  categoryName: string
}

export interface Product {
  productId: number
  categoryNumber: number
  productName: string
  characteristics: string
}

export interface StoreProduct {
  upc: string
  upcProm?: string | null
  productId: number
  sellingPrice: number
  productsNumber: number
  promotionalProduct: boolean
}

export interface StoreProductDetails {
  upc: string
  upcProm?: string | null
  productId: number
  productName: string
  characteristics: string
  categoryNumber: number
  sellingPrice: number
  productsNumber: number
  promotionalProduct: boolean
}

export interface CustomerCard {
  cardNumber: string
  surname: string
  firstname: string
  patronymic?: string | null
  phoneNumber: string
  city?: string | null
  street?: string | null
  zipCode?: string | null
  percent: number
}

export interface StoreCheck {
  checkNumber: string
  employeeId: string
  cardNumber?: string | null
  printDate: string
  sumTotal: number
  vat: number
}

export interface SaleDetails {
  upc: string
  productName: string
  productNumber: number
  sellingPrice: number
}

export interface CheckDetails extends StoreCheck {
  items: SaleDetails[]
}

export interface ProductRevenue {
  productName: string
  totalQuantity: number
  totalRevenue: number
}

export interface CategoryBuyer {
  cardNumber: string
  surname: string
  firstname: string
}

export interface CategoryRevenue {
  categoryName: string
  totalRevenue: number
}

export interface CashierPromo {
  employeeId: string
  surname: string
  firstname: string
}
