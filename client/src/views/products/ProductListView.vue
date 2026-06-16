<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Select from 'primevue/select'
import IconField from 'primevue/iconfield'
import InputIcon from 'primevue/inputicon'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { productsApi, type ProductPayload } from '@/api/products'
import { categoriesApi } from '@/api/categories'
import { describeError } from '@/utils/apiError'
import { printReport } from '@/utils/printReport'
import { useAuthStore } from '@/stores/auth'
import type { Category, Product } from '@/types'

const auth = useAuthStore()
const toast = useToast()
const confirm = useConfirm()

const products = ref<Product[]>([])
const categories = ref<Category[]>([])
const loading = ref(false)
const search = ref('')
const categoryFilter = ref<number | null>(null)

const dialogVisible = ref(false)
const editing = ref<Product | null>(null)
const form = ref<ProductPayload>({ categoryNumber: 0, productName: '', characteristics: '' })

const categoryName = (n: number) =>
  categories.value.find((c) => c.categoryNumber === n)?.categoryName ?? `#${n}`

const categoryOptions = computed(() =>
  categories.value.map((c) => ({ label: c.categoryName, value: c.categoryNumber })),
)

let searchTimer: number | undefined
watch(search, () => {
  window.clearTimeout(searchTimer)
  searchTimer = window.setTimeout(load, 300)
})

async function load() {
  loading.value = true
  try {
    products.value = await productsApi.getAll({
      search: search.value.trim() || undefined,
      categoryNumber: categoryFilter.value ?? undefined,
    })
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося завантажити'), life: 4000 })
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editing.value = null
  form.value = { categoryNumber: categories.value[0]?.categoryNumber ?? 0, productName: '', characteristics: '' }
  dialogVisible.value = true
}

function openEdit(p: Product) {
  editing.value = p
  form.value = { categoryNumber: p.categoryNumber, productName: p.productName, characteristics: p.characteristics }
  dialogVisible.value = true
}

async function save() {
  if (!form.value.productName.trim()) {
    toast.add({ severity: 'warn', summary: 'Вкажіть назву товару', life: 2500 })
    return
  }
  try {
    if (editing.value) await productsApi.update(editing.value.productId, form.value)
    else await productsApi.create(form.value)
    toast.add({ severity: 'success', summary: 'Збережено', life: 2000 })
    dialogVisible.value = false
    await load()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося зберегти'), life: 4000 })
  }
}

function confirmDelete(p: Product) {
  confirm.require({
    message: `Видалити товар «${p.productName}»?`,
    header: 'Підтвердження',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Видалити',
    rejectLabel: 'Скасувати',
    acceptProps: { severity: 'danger' },
    accept: async () => {
      try {
        await productsApi.remove(p.productId)
        toast.add({ severity: 'success', summary: 'Видалено', life: 2000 })
        await load()
      } catch (e) {
        toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося видалити (можливо, є товари в магазині)'), life: 5000 })
      }
    },
  })
}

function printAll() {
  printReport('Звіт: товари', [
    { header: 'ID', value: (p: Product) => p.productId },
    { header: 'Назва', value: (p: Product) => p.productName },
    { header: 'Категорія', value: (p: Product) => categoryName(p.categoryNumber) },
    { header: 'Характеристики', value: (p: Product) => p.characteristics },
  ], products.value)
}

onMounted(async () => {
  categories.value = await categoriesApi.getAll().catch(() => [])
  await load()
})
</script>

<template>
  <div class="page">
    <div class="page-header">
      <h1>Товари</h1>
      <div class="toolbar">
        <Select v-model="categoryFilter" :options="categoryOptions" option-label="label" option-value="value"
          placeholder="Усі категорії" show-clear @change="load" />
        <IconField>
          <InputIcon class="pi pi-search" />
          <InputText v-model="search" placeholder="Знайти за назвою" />
        </IconField>
        <Button v-if="auth.isManager" label="Друк" icon="pi pi-print" severity="secondary" outlined @click="printAll" />
        <Button v-if="auth.isManager" label="Додати" icon="pi pi-plus" @click="openCreate" />
      </div>
    </div>

    <DataTable :value="products" :loading="loading" paginator :rows="10" striped-rows data-key="productId"
      sort-field="productName" :sort-order="1">
      <Column field="productId" header="ID" sortable :style="{ width: '5rem' }" />
      <Column field="productName" header="Назва" sortable />
      <Column header="Категорія" sortable>
        <template #body="{ data }">{{ categoryName(data.categoryNumber) }}</template>
      </Column>
      <Column field="characteristics" header="Характеристики" />
      <Column v-if="auth.isManager" header="" :style="{ width: '7rem' }">
        <template #body="{ data }">
          <Button icon="pi pi-pencil" text rounded @click="openEdit(data)" />
          <Button icon="pi pi-trash" text rounded severity="danger" @click="confirmDelete(data)" />
        </template>
      </Column>
    </DataTable>

    <Dialog v-model:visible="dialogVisible" :header="editing ? 'Редагувати товар' : 'Новий товар'" modal
      :style="{ width: '500px' }">
      <div class="form">
        <label>
          <span>Категорія</span>
          <Select v-model="form.categoryNumber" :options="categoryOptions" option-label="label" option-value="value" fluid />
        </label>
        <label>
          <span>Назва</span>
          <InputText v-model="form.productName" fluid />
        </label>
        <label>
          <span>Характеристики</span>
          <Textarea v-model="form.characteristics" rows="3" fluid />
        </label>
      </div>
      <template #footer>
        <Button label="Скасувати" severity="secondary" text @click="dialogVisible = false" />
        <Button label="Зберегти" icon="pi pi-check" @click="save" />
      </template>
    </Dialog>
  </div>
</template>

<style scoped>
.form {
  display: flex;
  flex-direction: column;
  gap: 0.9rem;
}
.form label {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
  font-size: 0.85rem;
}
</style>
