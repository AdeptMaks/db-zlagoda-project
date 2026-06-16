import api from './client'
import type {
  CreateEmployeeRequest,
  Employee,
  EmployeeContact,
  UpdateEmployeeRequest,
} from '@/types'

export const employeesApi = {
  getAll: () => api.get<Employee[]>('/employees').then((r) => r.data),

  getCashiers: () => api.get<Employee[]>('/employees/cashiers').then((r) => r.data),

  getById: (id: string) => api.get<Employee>(`/employees/${id}`).then((r) => r.data),

  getMe: () => api.get<Employee>('/employees/me').then((r) => r.data),

  searchBySurname: (surname: string) =>
    api
      .get<EmployeeContact[]>('/employees/search', { params: { surname } })
      .then((r) => r.data),

  create: (body: CreateEmployeeRequest) =>
    api.post<Employee>('/employees', body).then((r) => r.data),

  update: (id: string, body: UpdateEmployeeRequest) =>
    api.put<Employee>(`/employees/${id}`, body).then((r) => r.data),

  remove: (id: string) => api.delete(`/employees/${id}`).then((r) => r.data),
}
