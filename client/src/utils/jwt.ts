import type { EmployeeRole } from '@/types'

export interface DecodedToken {
  name: string | null
  role: EmployeeRole | null
  exp: number | null
}

const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

function base64UrlDecode(segment: string): string {
  const padded = segment.replace(/-/g, '+').replace(/_/g, '/')
  const decoded = atob(padded)
  return decodeURIComponent(
    decoded
      .split('')
      .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
      .join(''),
  )
}

export function defineComputedRole(token: string | null): DecodedToken | null {
  if (!token) return null
  const parts = token.split('.')
  if (parts.length < 2) return null

  try {
    const payload = JSON.parse(base64UrlDecode(parts[1])) as Record<string, unknown>
    const name = (payload[NAME_CLAIM] ?? payload['name'] ?? payload['unique_name']) as
      | string
      | undefined
    const role = (payload[ROLE_CLAIM] ?? payload['role']) as string | undefined
    const exp = typeof payload['exp'] === 'number' ? (payload['exp'] as number) : null

    return {
      name: name ?? null,
      role: (role as EmployeeRole) ?? null,
      exp,
    }
  } catch {
    return null
  }
}
