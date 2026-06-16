<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import Dialog from 'primevue/dialog'
import Select from 'primevue/select'
import InputNumber from 'primevue/inputnumber'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Message from 'primevue/message'
import { useToast } from 'primevue/usetoast'
import { storeProductsApi } from '@/api/storeProducts'
import { customerCardsApi } from '@/api/customerCards'
import { checksApi } from '@/api/checks'
import { describeError } from '@/utils/apiError'
import type { CustomerCard, StoreProductDetails } from '@/types'

const props = defineProps<{ visible: boolean }>()
const emit = defineEmits<{ 'update:visible': [v: boolean]; created: [] }>()

const toast = useToast()

const storeProducts = ref<StoreProductDetails[]>([])
const cards = ref<CustomerCard[]>([])
const cardNumber = ref<string | null>(null)
const selectedUpc = ref<string | null>(null)
const quantity = ref<number>(1)
const lines = ref<{ upc: string; name: string; price: number; quantity: number }[]>([])
const saving = ref(false)

const productOptions = computed(() =>
  storeProducts.value.map((sp) => ({
    label: `${sp.productName} — ${sp.upc} (${sp.sellingPrice.toFixed(2)}, залишок: ${sp.productsNumber})`,
    value: sp.upc,
  })),
)
const cardOptions = computed(() =>
  cards.value.map((c) => ({ label: `${c.cardNumber} — ${c.surname} (${c.percent}%)`, value: c.cardNumber })),
)

const selectedCard = computed(() => cards.value.find((c) => c.cardNumber === cardNumber.value) ?? null)
const rawTotal = computed(() => lines.value.reduce((s, l) => s + l.price * l.quantity, 0))
const discount = computed(() => selectedCard.value?.percent ?? 0)
const sumTotal = computed(() => rawTotal.value * (1 - discount.value / 100))
const vat = computed(() => sumTotal.value * 0.2)

watch(
  () => props.visible,
  async (open) => {
    if (!open) return
    cardNumber.value = null
    selectedUpc.value = null
    quantity.value = 1
    lines.value = []
    storeProducts.value = await storeProductsApi.getAll().catch(() => [])
    cards.value = await customerCardsApi.getAll().catch(() => [])
  },
)

function addLine() {
  if (!selectedUpc.value || quantity.value <= 0) return
  const sp = storeProducts.value.find((s) => s.upc === selectedUpc.value)
  if (!sp) return
  const existing = lines.value.find((l) => l.upc === sp.upc)
  if (existing) existing.quantity += quantity.value
  else lines.value.push({ upc: sp.upc, name: sp.productName, price: sp.sellingPrice, quantity: quantity.value })
  selectedUpc.value = null
  quantity.value = 1
}

function removeLine(upc: string) {
  lines.value = lines.value.filter((l) => l.upc !== upc)
}

async function submit() {
  if (lines.value.length === 0) {
    toast.add({ severity: 'warn', summary: 'Додайте хоча б один товар', life: 2500 })
    return
  }
  saving.value = true
  try {
    await checksApi.create({
      cardNumber: cardNumber.value || null,
      items: lines.value.map((l) => ({ upc: l.upc, quantity: l.quantity })),
    })
    toast.add({ severity: 'success', summary: 'Чек створено', life: 2500 })
    emit('created')
    emit('update:visible', false)
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося створити чек'), life: 5000 })
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <Dialog :visible="visible" header="Новий чек" modal :style="{ width: '640px' }"
    @update:visible="emit('update:visible', $event)">
    <div class="block">
      <label>
        <span>Карта клієнта (необовʼязково)</span>
        <Select v-model="cardNumber" :options="cardOptions" option-label="label" option-value="value"
          filter show-clear placeholder="— без картки —" fluid />
      </label>
    </div>

    <div class="add-row">
      <Select v-model="selectedUpc" :options="productOptions" option-label="label" option-value="value"
        filter placeholder="Оберіть товар" class="grow" />
      <InputNumber v-model="quantity" :min="1" :style="{ width: '6rem' }" />
      <Button label="Додати" icon="pi pi-plus" @click="addLine" />
    </div>

    <DataTable :value="lines" data-key="upc" class="lines">
      <Column field="name" header="Товар" />
      <Column field="price" header="Ціна">
        <template #body="{ data }">{{ data.price.toFixed(2) }}</template>
      </Column>
      <Column field="quantity" header="К-сть" />
      <Column header="Сума">
        <template #body="{ data }">{{ (data.price * data.quantity).toFixed(2) }}</template>
      </Column>
      <Column :style="{ width: '3rem' }">
        <template #body="{ data }">
          <Button icon="pi pi-times" text rounded severity="danger" @click="removeLine(data.upc)" />
        </template>
      </Column>
    </DataTable>

    <Message severity="secondary" :closable="false" class="totals">
      <div class="totals-grid">
        <span>Без знижки:</span><strong>{{ rawTotal.toFixed(2) }}</strong>
        <span>Знижка:</span><strong>{{ discount }}%</strong>
        <span>До сплати:</span><strong>{{ sumTotal.toFixed(2) }}</strong>
        <span>ПДВ (у т.ч.):</span><strong>{{ vat.toFixed(2) }}</strong>
      </div>
    </Message>

    <template #footer>
      <Button label="Скасувати" severity="secondary" text @click="emit('update:visible', false)" />
      <Button label="Створити чек" icon="pi pi-check" :loading="saving" @click="submit" />
    </template>
  </Dialog>
</template>

<style scoped>
.block label { display: flex; flex-direction: column; gap: 0.35rem; font-size: 0.85rem; margin-bottom: 0.75rem; }
.add-row { display: flex; gap: 0.5rem; align-items: center; margin-bottom: 0.75rem; }
.grow { flex: 1; }
.lines { margin-bottom: 0.75rem; }
.totals-grid { display: grid; grid-template-columns: auto 1fr; gap: 0.25rem 1rem; }
</style>
