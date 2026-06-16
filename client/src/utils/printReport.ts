export interface ReportColumn<T> {
  header: string
  value: (row: T) => string | number | null | undefined
}

function escapeHtml(value: unknown): string {
  return String(value ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
}

export function printReport<T>(title: string, columns: ReportColumn<T>[], rows: T[]): void {
  const now = new Date().toLocaleString('uk-UA')

  const head = columns.map((c) => `<th>${escapeHtml(c.header)}</th>`).join('')
  const body = rows
    .map(
      (row) =>
        `<tr>${columns.map((c) => `<td>${escapeHtml(c.value(row))}</td>`).join('')}</tr>`,
    )
    .join('')

  const html = `<!doctype html>
<html lang="uk"><head><meta charset="utf-8"><title>${escapeHtml(title)}</title>
<style>
  body { font-family: system-ui, sans-serif; padding: 24px; color: #111; }
  h1 { font-size: 18px; margin: 0 0 4px; }
  .meta { color: #666; font-size: 12px; margin-bottom: 16px; }
  table { width: 100%; border-collapse: collapse; font-size: 12px; }
  th, td { border: 1px solid #999; padding: 6px 8px; text-align: left; }
  th { background: #eee; }
  tr:nth-child(even) td { background: #f6f6f6; }
</style></head>
<body>
  <h1>${escapeHtml(title)}</h1>
  <div class="meta">ZLAGODA · сформовано ${escapeHtml(now)} · записів: ${rows.length}</div>
  <table><thead><tr>${head}</tr></thead><tbody>${body}</tbody></table>
</body></html>`

  const iframe = document.createElement('iframe')
  iframe.style.position = 'fixed'
  iframe.style.right = '0'
  iframe.style.bottom = '0'
  iframe.style.width = '0'
  iframe.style.height = '0'
  iframe.style.border = '0'
  document.body.appendChild(iframe)

  const doc = iframe.contentWindow?.document
  if (!doc) {
    document.body.removeChild(iframe)
    return
  }
  doc.open()
  doc.write(html)
  doc.close()

  const win = iframe.contentWindow!
  win.focus()
  win.onafterprint = () => document.body.removeChild(iframe)
  setTimeout(() => win.print(), 100)
}
