<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import DatePicker from 'primevue/datepicker'
import Select from 'primevue/select'
import Dialog from 'primevue/dialog'
import Card from 'primevue/card'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { checksApi, type CheckFilter } from '@/api/checks'
import { employeesApi } from '@/api/employees'
import { describeError } from '@/utils/apiError'
import { printReport } from '@/utils/printReport'
import { useAuthStore } from '@/stores/auth'
import type { CheckDetails, Employee, StoreCheck } from '@/types'
import CheckCreateDialog from './CheckCreateDialog.vue'

const auth = useAuthStore()
const toast = useToast()
const confirm = useConfirm()

const checks = ref<StoreCheck[]>([])
const cashiers = ref<Employee[]>([])
const loading = ref(false)
const totalSum = ref<number>(0)

const from = ref<Date | null>(null)
const to = ref<Date | null>(null)
const employeeId = ref<string | null>(null)

const createVisible = ref(false)
const detailsVisible = ref(false)
const details = ref<CheckDetails | null>(null)

const cashierOptions = computed(() =>
  cashiers.value.map((c) => ({ label: `${c.surname} ${c.firstname}`, value: c.employeeId })),
)

function fmt(d: Date, endOfDay = false): string {
  const day = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
  return `${day}T${endOfDay ? '23:59:59' : '00:00:00'}`
}

function currentFilter(): CheckFilter {
  return {
    employeeId: auth.isManager ? employeeId.value || undefined : undefined,
    from: from.value ? fmt(from.value) : undefined,
    to: to.value ? fmt(to.value, true) : undefined,
  }
}

async function load() {
  loading.value = true
  try {
    const filter = currentFilter()
    const [list, sum] = await Promise.all([checksApi.getAll(filter), checksApi.totalSum(filter)])
    checks.value = list
    totalSum.value = sum.total
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося завантажити чеки'), life: 4000 })
  } finally {
    loading.value = false
  }
}

function today() {
  const d = new Date()
  from.value = d
  to.value = d
  load()
}

function resetFilters() {
  from.value = null
  to.value = null
  employeeId.value = null
  load()
}

async function openDetails(c: StoreCheck) {
  try {
    details.value = await checksApi.getById(c.checkNumber)
    detailsVisible.value = true
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося завантажити чек'), life: 4000 })
  }
}

function confirmDelete(c: StoreCheck) {
  confirm.require({
    message: `Видалити чек ${c.checkNumber}?`,
    header: 'Підтвердження',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Видалити',
    rejectLabel: 'Скасувати',
    acceptProps: { severity: 'danger' },
    accept: async () => {
      try {
        await checksApi.remove(c.checkNumber)
        toast.add({ severity: 'success', summary: 'Видалено', life: 2000 })
        await load()
      } catch (e) {
        toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося видалити'), life: 4000 })
      }
    },
  })
}

const formatDateTime = (s: string) => new Date(s).toLocaleString('uk-UA')

function printAll() {
  printReport(`Звіт: чеки (на суму ${totalSum.value.toFixed(2)} грн)`, [
    { header: '№ чека', value: (c: StoreCheck) => c.checkNumber },
    { header: 'Дата/час', value: (c: StoreCheck) => formatDateTime(c.printDate) },
    { header: 'Касир (ID)', value: (c: StoreCheck) => c.employeeId },
    { header: 'Картка', value: (c: StoreCheck) => c.cardNumber ?? '—' },
    { header: 'Сума', value: (c: StoreCheck) => c.sumTotal.toFixed(2) },
    { header: 'ПДВ', value: (c: StoreCheck) => c.vat.toFixed(2) },
  ], checks.value)
}

onMounted(async () => {
  if (auth.isManager) cashiers.value = await employeesApi.getCashiers().catch(() => [])
  await load()
})
</script>

<template>
  <div class="page">
    <div class="page-header">
      <h1>Чеки</h1>
      <div class="toolbar">
        <DatePicker v-model="from" date-format="yy-mm-dd" placeholder="Від" show-icon @update:model-value="load" />
        <DatePicker v-model="to" date-format="yy-mm-dd" placeholder="До" show-icon @update:model-value="load" />
        <Select v-if="auth.isManager" v-model="employeeId" :options="cashierOptions" option-label="label"
          option-value="value" placeholder="Усі касири" show-clear @change="load" />
        <Button label="Сьогодні" icon="pi pi-calendar" severity="secondary" @click="today" />
        <Button icon="pi pi-filter-slash" severity="secondary" outlined @click="resetFilters" />
        <Button v-if="auth.isManager" label="Друк" icon="pi pi-print" severity="secondary" outlined @click="printAll" />
        <Button v-if="auth.isCashier" label="Створити чек" icon="pi pi-plus" @click="createVisible = true" />
      </div>
    </div>

    <Card class="stats">
      <template #content>
        <span>Сума продажів за вибраний період: </span>
        <strong>{{ totalSum.toFixed(2) }} грн</strong>
        <span class="muted"> · чеків: {{ checks.length }}</span>
      </template>
    </Card>

    <DataTable :value="checks" :loading="loading" paginator :rows="15" striped-rows data-key="checkNumber">
      <Column field="checkNumber" header="№ чека" sortable />
      <Column field="printDate" header="Дата/час" sortable>
        <template #body="{ data }">{{ formatDateTime(data.printDate) }}</template>
      </Column>
      <Column field="employeeId" header="Касир (ID)" sortable />
      <Column field="cardNumber" header="Картка">
        <template #body="{ data }">{{ data.cardNumber ?? '—' }}</template>
      </Column>
      <Column field="sumTotal" header="Сума" sortable>
        <template #body="{ data }">{{ data.sumTotal.toFixed(2) }}</template>
      </Column>
      <Column field="vat" header="ПДВ" sortable>
        <template #body="{ data }">{{ data.vat.toFixed(2) }}</template>
      </Column>
      <Column header="" :style="{ width: '7rem' }">
        <template #body="{ data }">
          <Button icon="pi pi-eye" text rounded @click="openDetails(data)" />
          <Button v-if="auth.isManager" icon="pi pi-trash" text rounded severity="danger" @click="confirmDelete(data)" />
        </template>
      </Column>
    </DataTable>

    <CheckCreateDialog v-model:visible="createVisible" @created="load" />

    <Dialog v-model:visible="detailsVisible" :header="`Чек ${details?.checkNumber ?? ''}`" modal :style="{ width: '600px' }">
      <template v-if="details">
        <div class="meta">
          <div>Дата: {{ formatDateTime(details.printDate) }}</div>
          <div>Касир: {{ details.employeeId }}</div>
          <div>Картка: {{ details.cardNumber ?? '—' }}</div>
        </div>
        <DataTable :value="details.items" data-key="upc">
          <Column field="productName" header="Товар" />
          <Column field="sellingPrice" header="Ціна">
            <template #body="{ data }">{{ data.sellingPrice.toFixed(2) }}</template>
          </Column>
          <Column field="productNumber" header="К-сть" />
          <Column header="Сума">
            <template #body="{ data }">{{ (data.sellingPrice * data.productNumber).toFixed(2) }}</template>
          </Column>
        </DataTable>
        <div class="totals">
          <div>До сплати: <strong>{{ details.sumTotal.toFixed(2) }} грн</strong></div>
          <div class="muted">ПДВ (у т.ч.): {{ details.vat.toFixed(2) }} грн</div>
        </div>
      </template>
    </Dialog>
  </div>
</template>

<style scoped>
.stats { margin-bottom: 1rem; }
.muted { color: #6b7280; }
.meta { display: flex; gap: 1.5rem; margin-bottom: 1rem; font-size: 0.9rem; flex-wrap: wrap; }
.totals { margin-top: 1rem; text-align: right; }
</style>
