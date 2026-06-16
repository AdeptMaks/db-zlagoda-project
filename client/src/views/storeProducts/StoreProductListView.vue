<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import Select from 'primevue/select'
import SelectButton from 'primevue/selectbutton'
import ToggleSwitch from 'primevue/toggleswitch'
import Tag from 'primevue/tag'
import Message from 'primevue/message'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { storeProductsApi, type StoreProductPayload } from '@/api/storeProducts'
import { productsApi } from '@/api/products'
import { describeError } from '@/utils/apiError'
import { printReport } from '@/utils/printReport'
import { useAuthStore } from '@/stores/auth'
import type { Product, StoreProductDetails } from '@/types'

const auth = useAuthStore()
const toast = useToast()
const confirm = useConfirm()

const items = ref<StoreProductDetails[]>([])
const products = ref<Product[]>([])
const loading = ref(false)
const sort = ref<'name' | 'quantity'>('name')
const promoFilter = ref<'all' | 'promo' | 'regular'>('all')

const sortOptions = [
  { label: 'За назвою', value: 'name' },
  { label: 'За кількістю', value: 'quantity' },
]
const promoOptions = [
  { label: 'Усі', value: 'all' },
  { label: 'Акційні', value: 'promo' },
  { label: 'Звичайні', value: 'regular' },
]

const dialogVisible = ref(false)
const editing = ref<StoreProductDetails | null>(null)
const form = ref<StoreProductPayload>({
  upc: '', upcProm: null, productId: 0, sellingPrice: 0, productsNumber: 0, promotionalProduct: false,
})

const productOptions = computed(() =>
  products.value.map((p) => ({ label: `${p.productName} (#${p.productId})`, value: p.productId })),
)
const promLinkOptions = computed(() =>
  items.value
    .filter((i) => !i.promotionalProduct && i.upc !== form.value.upc)
    .map((i) => ({ label: `${i.upc} — ${i.productName}`, value: i.upc })),
)

watch([sort, promoFilter], load)

async function load() {
  loading.value = true
  try {
    const promotional =
      promoFilter.value === 'all' ? undefined : promoFilter.value === 'promo'
    items.value = await storeProductsApi.getAll({ sort: sort.value, promotional })
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося завантажити'), life: 4000 })
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editing.value = null
  form.value = {
    upc: '', upcProm: null, productId: products.value[0]?.productId ?? 0,
    sellingPrice: 0, productsNumber: 0, promotionalProduct: false,
  }
  dialogVisible.value = true
}

function openEdit(sp: StoreProductDetails) {
  editing.value = sp
  form.value = {
    upc: sp.upc, upcProm: sp.upcProm ?? null, productId: sp.productId,
    sellingPrice: sp.sellingPrice, productsNumber: sp.productsNumber,
    promotionalProduct: sp.promotionalProduct,
  }
  dialogVisible.value = true
}

async function save() {
  if (!form.value.upc.trim() || form.value.upc.length > 12) {
    toast.add({ severity: 'warn', summary: 'UPC обовʼязковий (до 12 символів)', life: 2500 })
    return
  }
  try {
    if (editing.value) await storeProductsApi.update(editing.value.upc, form.value)
    else await storeProductsApi.create(form.value)
    toast.add({ severity: 'success', summary: 'Збережено', life: 2000 })
    dialogVisible.value = false
    await load()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося зберегти'), life: 4000 })
  }
}

function confirmDelete(sp: StoreProductDetails) {
  confirm.require({
    message: `Видалити товар у магазині ${sp.upc} (${sp.productName})?`,
    header: 'Підтвердження',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Видалити',
    rejectLabel: 'Скасувати',
    acceptProps: { severity: 'danger' },
    accept: async () => {
      try {
        await storeProductsApi.remove(sp.upc)
        toast.add({ severity: 'success', summary: 'Видалено', life: 2000 })
        await load()
      } catch (e) {
        toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося видалити (можливо, є продажі)'), life: 5000 })
      }
    },
  })
}

function printAll() {
  const promoLabel = promoFilter.value === 'promo' ? ' (акційні)' : promoFilter.value === 'regular' ? ' (звичайні)' : ''
  printReport(`Звіт: товари в магазині${promoLabel}`, [
    { header: 'UPC', value: (s: StoreProductDetails) => s.upc },
    { header: 'Товар', value: (s: StoreProductDetails) => s.productName },
    { header: 'Ціна', value: (s: StoreProductDetails) => s.sellingPrice.toFixed(2) },
    { header: 'К-сть', value: (s: StoreProductDetails) => s.productsNumber },
    { header: 'Тип', value: (s: StoreProductDetails) => (s.promotionalProduct ? 'Акційний' : 'Звичайний') },
    { header: 'Характеристики', value: (s: StoreProductDetails) => s.characteristics },
  ], items.value)
}

onMounted(async () => {
  products.value = await productsApi.getAll().catch(() => [])
  await load()
})
</script>

<template>
  <div class="page">
    <div class="page-header">
      <h1>Товари в магазині</h1>
      <div class="toolbar">
        <SelectButton v-model="promoFilter" :options="promoOptions" option-label="label" option-value="value" :allow-empty="false" />
        <SelectButton v-model="sort" :options="sortOptions" option-label="label" option-value="value" :allow-empty="false" />
        <Button v-if="auth.isManager" label="Друк" icon="pi pi-print" severity="secondary" outlined @click="printAll" />
        <Button v-if="auth.isManager" label="Додати" icon="pi pi-plus" @click="openCreate" />
      </div>
    </div>

    <DataTable :value="items" :loading="loading" paginator :rows="10" striped-rows data-key="upc">
      <Column field="upc" header="UPC" sortable />
      <Column field="productName" header="Товар" sortable />
      <Column field="sellingPrice" header="Ціна" sortable>
        <template #body="{ data }">{{ data.sellingPrice.toFixed(2) }}</template>
      </Column>
      <Column field="productsNumber" header="К-сть" sortable />
      <Column field="promotionalProduct" header="Тип">
        <template #body="{ data }">
          <Tag :value="data.promotionalProduct ? 'Акційний' : 'Звичайний'"
            :severity="data.promotionalProduct ? 'warn' : 'secondary'" />
        </template>
      </Column>
      <Column field="characteristics" header="Характеристики" />
      <Column v-if="auth.isManager" header="" :style="{ width: '7rem' }">
        <template #body="{ data }">
          <Button icon="pi pi-pencil" text rounded @click="openEdit(data)" />
          <Button icon="pi pi-trash" text rounded severity="danger" @click="confirmDelete(data)" />
        </template>
      </Column>
    </DataTable>

    <Dialog v-model:visible="dialogVisible" :header="editing ? 'Редагувати товар у магазині' : 'Новий товар у магазині'" modal
      :style="{ width: '520px' }">
      <div class="form">
        <label>
          <span>UPC</span>
          <InputText v-model="form.upc" :disabled="!!editing" fluid />
        </label>
        <label>
          <span>Товар</span>
          <Select v-model="form.productId" :options="productOptions" option-label="label" option-value="value" filter fluid />
        </label>
        <div class="row">
          <label>
            <span>Ціна продажу</span>
            <InputNumber v-model="form.sellingPrice" mode="decimal" :min-fraction-digits="2" :min="0" fluid />
          </label>
          <label>
            <span>Кількість одиниць</span>
            <InputNumber v-model="form.productsNumber" :min="0" fluid />
          </label>
        </div>
        <label class="switch">
          <ToggleSwitch v-model="form.promotionalProduct" />
          <span>Акційний товар</span>
        </label>
        <label v-if="form.promotionalProduct">
          <span>Базовий товар (UPC)</span>
          <Select v-model="form.upcProm" :options="promLinkOptions" option-label="label" option-value="value"
            show-clear placeholder="— не звʼязувати —" fluid />
        </label>
        <Message v-if="form.promotionalProduct && form.upcProm" severity="info" :closable="false">
          Ціну буде перераховано як 80% від ціни базового товару.
        </Message>
      </div>
      <template #footer>
        <Button label="Скасувати" severity="secondary" text @click="dialogVisible = false" />
        <Button label="Зберегти" icon="pi pi-check" @click="save" />
      </template>
    </Dialog>
  </div>
</template>

<style scoped>
.form { display: flex; flex-direction: column; gap: 0.9rem; }
.form label { display: flex; flex-direction: column; gap: 0.35rem; font-size: 0.85rem; }
.row { display: grid; grid-template-columns: 1fr 1fr; gap: 0.9rem; }
.switch { flex-direction: row !important; align-items: center; gap: 0.6rem; }
</style>
