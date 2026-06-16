<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import IconField from 'primevue/iconfield'
import InputIcon from 'primevue/inputicon'
import Message from 'primevue/message'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { customerCardsApi, type CustomerCardPayload } from '@/api/customerCards'
import { describeError } from '@/utils/apiError'
import { printReport } from '@/utils/printReport'
import { useAuthStore } from '@/stores/auth'
import type { CustomerCard } from '@/types'

const auth = useAuthStore()
const toast = useToast()
const confirm = useConfirm()

const cards = ref<CustomerCard[]>([])
const loading = ref(false)
const search = ref('')

const dialogVisible = ref(false)
const editing = ref<CustomerCard | null>(null)
const errors = ref<string[]>([])
const form = ref<CustomerCardPayload>({
  surname: '', firstname: '', patronymic: '', phoneNumber: '', city: '', street: '', zipCode: '', percent: 0,
})

let searchTimer: number | undefined
watch(search, () => {
  window.clearTimeout(searchTimer)
  searchTimer = window.setTimeout(load, 300)
})

async function load() {
  loading.value = true
  try {
    cards.value = await customerCardsApi.getAll({ search: search.value.trim() || undefined })
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося завантажити'), life: 4000 })
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editing.value = null
  errors.value = []
  form.value = { surname: '', firstname: '', patronymic: '', phoneNumber: '', city: '', street: '', zipCode: '', percent: 0 }
  dialogVisible.value = true
}

function openEdit(c: CustomerCard) {
  editing.value = c
  errors.value = []
  form.value = {
    surname: c.surname, firstname: c.firstname, patronymic: c.patronymic ?? '',
    phoneNumber: c.phoneNumber, city: c.city ?? '', street: c.street ?? '',
    zipCode: c.zipCode ?? '', percent: c.percent,
  }
  dialogVisible.value = true
}

function validate(): boolean {
  const e: string[] = []
  if (!form.value.surname.trim()) e.push('Прізвище обовʼязкове.')
  if (!form.value.firstname.trim()) e.push("Імʼя обовʼязкове.")
  if (form.value.phoneNumber.trim().length === 0 || form.value.phoneNumber.length > 13)
    e.push('Телефон обовʼязковий (до 13 символів).')
  if (form.value.percent < 0 || form.value.percent > 100) e.push('Відсоток у межах 0–100.')
  if (form.value.zipCode && form.value.zipCode.length > 9) e.push('Індекс до 9 символів.')
  errors.value = e
  return e.length === 0
}

async function save() {
  if (!validate()) return
  const body: CustomerCardPayload = {
    ...form.value,
    patronymic: form.value.patronymic || null,
    city: form.value.city || null,
    street: form.value.street || null,
    zipCode: form.value.zipCode || null,
  }
  try {
    if (editing.value) await customerCardsApi.update(editing.value.cardNumber, body)
    else await customerCardsApi.create(body)
    toast.add({ severity: 'success', summary: 'Збережено', life: 2000 })
    dialogVisible.value = false
    await load()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося зберегти'), life: 4000 })
  }
}

function confirmDelete(c: CustomerCard) {
  confirm.require({
    message: `Видалити картку ${c.cardNumber} (${c.surname})?`,
    header: 'Підтвердження',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Видалити',
    rejectLabel: 'Скасувати',
    acceptProps: { severity: 'danger' },
    accept: async () => {
      try {
        await customerCardsApi.remove(c.cardNumber)
        toast.add({ severity: 'success', summary: 'Видалено', life: 2000 })
        await load()
      } catch (e) {
        toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося видалити'), life: 4000 })
      }
    },
  })
}

const fullName = (c: CustomerCard) => [c.surname, c.firstname, c.patronymic].filter(Boolean).join(' ')

function printAll() {
  printReport('Звіт: карти клієнтів', [
    { header: '№ картки', value: (c: CustomerCard) => c.cardNumber },
    { header: 'ПІБ', value: fullName },
    { header: 'Телефон', value: (c: CustomerCard) => c.phoneNumber },
    { header: 'Знижка %', value: (c: CustomerCard) => c.percent },
    { header: 'Адреса', value: (c: CustomerCard) => [c.city, c.street, c.zipCode].filter(Boolean).join(', ') },
  ], cards.value)
}

onMounted(load)
</script>

<template>
  <div class="page">
    <div class="page-header">
      <h1>Карти клієнтів</h1>
      <div class="toolbar">
        <IconField>
          <InputIcon class="pi pi-search" />
          <InputText v-model="search" placeholder="Пошук за прізвищем" />
        </IconField>
        <Button v-if="auth.isManager" label="Друк" icon="pi pi-print" severity="secondary" outlined @click="printAll" />
        <Button label="Додати" icon="pi pi-plus" @click="openCreate" />
      </div>
    </div>

    <DataTable :value="cards" :loading="loading" paginator :rows="10" striped-rows data-key="cardNumber"
      sort-field="surname" :sort-order="1">
      <Column field="cardNumber" header="№ картки" sortable />
      <Column header="ПІБ" sortable field="surname">
        <template #body="{ data }">{{ fullName(data) }}</template>
      </Column>
      <Column field="phoneNumber" header="Телефон" />
      <Column field="percent" header="Знижка %" sortable />
      <Column header="Адреса">
        <template #body="{ data }">{{ [data.city, data.street].filter(Boolean).join(', ') }}</template>
      </Column>
      <Column header="" :style="{ width: '7rem' }">
        <template #body="{ data }">
          <Button icon="pi pi-pencil" text rounded @click="openEdit(data)" />
          <Button v-if="auth.isManager" icon="pi pi-trash" text rounded severity="danger" @click="confirmDelete(data)" />
        </template>
      </Column>
    </DataTable>

    <Dialog v-model:visible="dialogVisible" :header="editing ? 'Редагувати картку' : 'Нова картка клієнта'" modal
      :style="{ width: '560px' }">
      <div class="form-grid">
        <label><span>Прізвище</span><InputText v-model="form.surname" fluid /></label>
        <label><span>Імʼя</span><InputText v-model="form.firstname" fluid /></label>
        <label class="span-2"><span>По батькові</span><InputText v-model="form.patronymic" fluid /></label>
        <label><span>Телефон</span><InputText v-model="form.phoneNumber" placeholder="+380..." fluid /></label>
        <label><span>Знижка %</span><InputNumber v-model="form.percent" :min="0" :max="100" fluid /></label>
        <label><span>Місто</span><InputText v-model="form.city" fluid /></label>
        <label><span>Вулиця</span><InputText v-model="form.street" fluid /></label>
        <label><span>Індекс</span><InputText v-model="form.zipCode" fluid /></label>
      </div>
      <Message v-if="errors.length" severity="error" :closable="false" class="errs">
        <ul><li v-for="(e, i) in errors" :key="i">{{ e }}</li></ul>
      </Message>
      <template #footer>
        <Button label="Скасувати" severity="secondary" text @click="dialogVisible = false" />
        <Button label="Зберегти" icon="pi pi-check" @click="save" />
      </template>
    </Dialog>
  </div>
</template>

<style scoped>
.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0.9rem; }
.form-grid label { display: flex; flex-direction: column; gap: 0.3rem; font-size: 0.85rem; }
.span-2 { grid-column: 1 / -1; }
.errs ul { margin: 0; padding-left: 1.1rem; }
</style>
