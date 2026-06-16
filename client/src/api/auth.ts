import api from './client'
import type { CreateEmployeeRequest, LoginRequest } from '@/types'

export const authApi = {
  login: (body: LoginRequest) =>
    api.post<string | { token: string }>('/auth/employee/login', body).then((r) => r.data),

  register: (body: CreateEmployeeRequest) =>
    api.post<string | { token: string }>('/auth/employee/register', body).then((r) => r.data),

  logout: () => api.post('/auth/employee/logout').then((r) => r.data),
}
