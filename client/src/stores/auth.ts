import { defineComputedRole, type DecodedToken } from '@/utils/jwt'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import type { EmployeeRole, LoginRequest } from '@/types'
import { authApi } from '@/api/auth'

const TOKEN_KEY = 'zlagoda.token'

function extractToken(data: string | { token: string }): string {
  return typeof data === 'string' ? data : data.token
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem(TOKEN_KEY))

  const decoded = computed<DecodedToken | null>(() => defineComputedRole(token.value))
  const role = computed<EmployeeRole | null>(() => decoded.value?.role ?? null)
  const username = computed<string | null>(() => decoded.value?.name ?? null)
  const isAuthenticated = computed(() => !!token.value)
  const isManager = computed(() => role.value === 'Manager')
  const isCashier = computed(() => role.value === 'Cachier')

  function setToken(value: string) {
    token.value = value
    localStorage.setItem(TOKEN_KEY, value)
  }

  async function login(credentials: LoginRequest) {
    const data = await authApi.login(credentials)
    const value = extractToken(data)
    if (!value) {
      throw new Error('Сервер не повернув токен. Логін на бекенді ще не реалізований.')
    }
    setToken(value)
  }

  function logout() {
    token.value = null
    localStorage.removeItem(TOKEN_KEY)
    void authApi.logout()
  }

  return {
    token,
    role,
    username,
    isAuthenticated,
    isManager,
    isCashier,
    setToken,
    login,
    logout,
  }
})
