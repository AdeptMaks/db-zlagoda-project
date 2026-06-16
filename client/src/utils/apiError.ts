import { AxiosError } from 'axios'

export function describeError(e: unknown, fallback: string): string {
  if (e instanceof AxiosError) {
    const data = e.response?.data
    if (typeof data === 'string' && data) return data
    if (data && typeof data === 'object') {
      if ('error' in data && typeof data.error === 'string') return data.error
      if (Array.isArray(data) && data.length && data[0]?.errorMessage)
        return data.map((x: { errorMessage: string }) => x.errorMessage).join('; ')
    }
    return `${fallback} (${e.response?.status ?? 'мережа'})`
  }
  return fallback
}
