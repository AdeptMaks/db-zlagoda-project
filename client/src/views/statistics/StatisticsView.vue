<script setup lang="ts">
import { onMounted, ref } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Card from 'primevue/card'
import DatePicker from 'primevue/datepicker'
import Select from 'primevue/select'
import { useToast } from 'primevue/usetoast'
import { statisticsApi } from '@/api/statistics'
import { employeesApi } from '@/api/employees'
import { categoriesApi } from '@/api/categories'
import { describeError } from '@/utils/apiError'
import type { CategoryBuyer, Category, Employee, ProductRevenue } from '@/types'

const toast = useToast()

const cashiers = ref<Employee[]>([])
const categories = ref<Category[]>([])

// ---- Запит 1: виторг за товарами (групування, параметричний) ----
const employeeId = ref<string | null>(null)
const from = ref<Date | null>(null)
const to = ref<Date | null>(null)
const revenue = ref<ProductRevenue[]>([])
const loadingRevenue = ref(false)

function fmt(d: Date, endOfDay = false): string {
  const day = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
  return `${day}T${endOfDay ? '23:59:59' : '00:00:00'}`
}

async function runRevenue() {
  loadingRevenue.value = true
  try {
    revenue.value = await statisticsApi.productRevenue({
      employeeId: employeeId.value || undefined,
      from: from.value ? fmt(from.value) : undefined,
      to: to.value ? fmt(to.value, true) : undefined,
    })
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося виконати запит'), life: 4000 })
  } finally {
    loadingRevenue.value = false
  }
}

// ---- Запит 2: клієнти, що купили всі товари категорії (подвійне заперечення) ----
const categoryNumber = ref<number | null>(null)
const buyers = ref<CategoryBuyer[]>([])
const loadingBuyers = ref(false)
const buyersRan = ref(false)

async function runBuyers() {
  if (categoryNumber.value === null) {
    toast.add({ severity: 'warn', summary: 'Оберіть категорію', life: 2500 })
    return
  }
  loadingBuyers.value = true
  try {
    buyers.value = await statisticsApi.categoryBuyers(categoryNumber.value)
    buyersRan.value = true
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося виконати запит'), life: 4000 })
  } finally {
    loadingBuyers.value = false
  }
}

const cashierOptions = () => cashiers.value.map((c) => ({ label: `${c.surname} ${c.firstname}`, value: c.employeeId }))
const categoryOptions = () => categories.value.map((c) => ({ label: c.categoryName, value: c.categoryNumber }))
const fullName = (b: CategoryBuyer) => `${b.surname} ${b.firstname}`

onMounted(async () => {
  cashiers.value = await employeesApi.getCashiers().catch(() => [])
  categories.value = await categoriesApi.getAll().catch(() => [])
  await runRevenue()
})
</script>

<template>
  <div class="page">
    <div class="page-header">
      <h1>Статистика</h1>
    </div>

    <Card class="block">
      <template #title>Виторг за товарами</template>
      <template #subtitle>
        Кількість проданих одиниць та сумарний виторг за кожним товаром.
        Фільтри (касир, період) — опціональні.
      </template>
      <template #content>
        <div class="toolbar">
          <Select v-model="employeeId" :options="cashierOptions()" option-label="label" option-value="value"
            placeholder="Усі касири" show-clear />
          <DatePicker v-model="from" date-format="yy-mm-dd" placeholder="Від" show-icon />
          <DatePicker v-model="to" date-format="yy-mm-dd" placeholder="До" show-icon />
          <Button label="Сформувати" icon="pi pi-chart-bar" @click="runRevenue" />
        </div>

        <DataTable :value="revenue" :loading="loadingRevenue" paginator :rows="10" striped-rows class="mt">
          <Column field="productName" header="Товар" sortable />
          <Column field="totalQuantity" header="Продано одиниць" sortable />
          <Column field="totalRevenue" header="Виторг, грн" sortable>
            <template #body="{ data }">{{ data.totalRevenue.toFixed(2) }}</template>
          </Column>
          <template #empty>Немає даних за вибраними умовами.</template>
        </DataTable>
      </template>
    </Card>

    <Card class="block">
      <template #title>Клієнти, що придбали всі товари категорії</template>
      <template #subtitle>
        Постійні клієнти, для яких немає жодного товару заданої категорії, який
        вони жодного разу не купували (подвійне заперечення).
      </template>
      <template #content>
        <div class="toolbar">
          <Select v-model="categoryNumber" :options="categoryOptions()" option-label="label" option-value="value"
            placeholder="Оберіть категорію" />
          <Button label="Сформувати" icon="pi pi-users" @click="runBuyers" />
        </div>

        <DataTable v-if="buyersRan" :value="buyers" :loading="loadingBuyers" paginator :rows="10" striped-rows class="mt">
          <Column field="cardNumber" header="№ картки" sortable />
          <Column header="ПІБ" sortable field="surname">
            <template #body="{ data }">{{ fullName(data) }}</template>
          </Column>
          <template #empty>Жоден клієнт не придбав усі товари цієї категорії.</template>
        </DataTable>
      </template>
    </Card>
  </div>
</template>

<style scoped>
.block { margin-bottom: 1.25rem; }
.mt { margin-top: 1rem; }
</style>
