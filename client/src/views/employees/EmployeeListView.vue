<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { AxiosError } from 'axios'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Tag from 'primevue/tag'
import IconField from 'primevue/iconfield'
import InputIcon from 'primevue/inputicon'
import SelectButton from 'primevue/selectbutton'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { employeesApi } from '@/api/employees'
import { printReport } from '@/utils/printReport'
import type { CreateEmployeeRequest, Employee, UpdateEmployeeRequest } from '@/types'
import EmployeeFormDialog from './EmployeeFormDialog.vue'

const toast = useToast()
const confirm = useConfirm()

const employees = ref<Employee[]>([])
const loading = ref(false)
const surnameFilter = ref('')
const roleFilter = ref<'all' | 'cashiers'>('all')
const roleOptions = [
  { label: 'Усі', value: 'all' },
  { label: 'Лише касири', value: 'cashiers' },
]

const dialogVisible = ref(false)
const editing = ref<Employee | null>(null)

const filtered = computed(() => {
  const q = surnameFilter.value.trim().toLowerCase()
  if (!q) return employees.value
  return employees.value.filter((e) => e.surname.toLowerCase().includes(q))
})

function describeError(e: unknown, fallback: string): string {
  if (e instanceof AxiosError) {
    const data = e.response?.data as { error?: string } | undefined
    return data?.error ?? `${fallback} (${e.response?.status ?? 'мережа'})`
  }
  return fallback
}

async function load() {
  loading.value = true
  try {
    employees.value =
      roleFilter.value === 'cashiers'
        ? await employeesApi.getCashiers()
        : await employeesApi.getAll()
  } catch (e) {
    toast.add({
      severity: 'error',
      summary: 'Помилка',
      detail: describeError(e, 'Не вдалося завантажити працівників'),
      life: 4000,
    })
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editing.value = null
  dialogVisible.value = true
}

function openEdit(employee: Employee) {
  editing.value = employee
  dialogVisible.value = true
}

async function handleCreate(body: CreateEmployeeRequest) {
  try {
    await employeesApi.create(body)
    toast.add({ severity: 'success', summary: 'Створено', life: 2500 })
    dialogVisible.value = false
    await load()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося створити'), life: 4000 })
  }
}

async function handleUpdate(id: string, body: UpdateEmployeeRequest) {
  try {
    await employeesApi.update(id, body)
    toast.add({ severity: 'success', summary: 'Оновлено', life: 2500 })
    dialogVisible.value = false
    await load()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося оновити'), life: 4000 })
  }
}

function confirmDelete(employee: Employee) {
  confirm.require({
    message: `Видалити працівника ${employee.surname} ${employee.firstname}?`,
    header: 'Підтвердження',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Видалити',
    rejectLabel: 'Скасувати',
    acceptProps: { severity: 'danger' },
    accept: async () => {
      try {
        await employeesApi.remove(employee.employeeId)
        toast.add({ severity: 'success', summary: 'Видалено', life: 2500 })
        await load()
      } catch (e) {
        toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося видалити'), life: 4000 })
      }
    },
  })
}

function fullName(e: Employee): string {
  return [e.surname, e.firstname, e.patronymic].filter(Boolean).join(' ')
}

function printAll() {
  printReport('Звіт: працівники', [
    { header: 'ID', value: (e: Employee) => e.employeeId },
    { header: 'ПІБ', value: fullName },
    { header: 'Роль', value: (e: Employee) => (e.employeeRole === 'Manager' ? 'Менеджер' : 'Касир') },
    { header: 'Зарплата', value: (e: Employee) => e.salary.toFixed(2) },
    { header: 'Телефон', value: (e: Employee) => e.phoneNumber },
    { header: 'Дата народження', value: (e: Employee) => formatDate(e.birthDate) },
    { header: 'Початок роботи', value: (e: Employee) => formatDate(e.startDate) },
    { header: 'Адреса', value: (e: Employee) => `${e.city}, ${e.street}, ${e.zipCode}` },
  ], filtered.value)
}

function formatDate(value: string): string {
  return value ? new Date(value).toLocaleDateString('uk-UA') : ''
}

onMounted(load)
</script>

<template>
  <div class="page">
    <div class="page-header">
      <h1>Працівники</h1>
      <div class="toolbar">
        <SelectButton
          v-model="roleFilter"
          :options="roleOptions"
          option-label="label"
          option-value="value"
          :allow-empty="false"
          @change="load"
        />
        <IconField>
          <InputIcon class="pi pi-search" />
          <InputText v-model="surnameFilter" placeholder="Пошук за прізвищем" />
        </IconField>
        <Button label="Друк" icon="pi pi-print" severity="secondary" outlined @click="printAll" />
        <Button label="Додати" icon="pi pi-plus" @click="openCreate" />
      </div>
    </div>

    <DataTable
      :value="filtered"
      :loading="loading"
      paginator
      :rows="10"
      :rows-per-page-options="[10, 20, 50]"
      sort-field="surname"
      :sort-order="1"
      striped-rows
      data-key="employeeId"
    >
      <Column field="surname" header="ПІБ" sortable>
        <template #body="{ data }">{{ fullName(data) }}</template>
      </Column>
      <Column field="employeeRole" header="Роль" sortable>
        <template #body="{ data }">
          <Tag
            :value="data.employeeRole === 'Manager' ? 'Менеджер' : 'Касир'"
            :severity="data.employeeRole === 'Manager' ? 'info' : 'success'"
          />
        </template>
      </Column>
      <Column field="salary" header="Зарплата" sortable>
        <template #body="{ data }">{{ data.salary?.toFixed(2) }}</template>
      </Column>
      <Column field="phoneNumber" header="Телефон" />
      <Column field="startDate" header="Початок роботи" sortable>
        <template #body="{ data }">{{ formatDate(data.startDate) }}</template>
      </Column>
      <Column header="Адреса">
        <template #body="{ data }">{{ data.city }}, {{ data.street }}</template>
      </Column>
      <Column header="" :style="{ width: '7rem' }">
        <template #body="{ data }">
          <div class="row-actions">
            <Button icon="pi pi-pencil" text rounded @click="openEdit(data)" />
            <Button icon="pi pi-trash" text rounded severity="danger" @click="confirmDelete(data)" />
          </div>
        </template>
      </Column>
    </DataTable>

    <EmployeeFormDialog
      v-model:visible="dialogVisible"
      :employee="editing"
      @save-create="handleCreate"
      @save-update="handleUpdate"
    />
  </div>
</template>

<style scoped>
.row-actions {
  display: flex;
  gap: 0.25rem;
}
</style>
