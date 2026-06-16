<script setup lang="ts">
import { onMounted, ref } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { categoriesApi } from '@/api/categories'
import { describeError } from '@/utils/apiError'
import { printReport } from '@/utils/printReport'
import type { Category } from '@/types'

const toast = useToast()
const confirm = useConfirm()

const categories = ref<Category[]>([])
const loading = ref(false)
const dialogVisible = ref(false)
const editing = ref<Category | null>(null)
const name = ref('')

async function load() {
  loading.value = true
  try {
    categories.value = await categoriesApi.getAll()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося завантажити'), life: 4000 })
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editing.value = null
  name.value = ''
  dialogVisible.value = true
}

function openEdit(c: Category) {
  editing.value = c
  name.value = c.categoryName
  dialogVisible.value = true
}

async function save() {
  if (!name.value.trim()) {
    toast.add({ severity: 'warn', summary: 'Вкажіть назву', life: 2500 })
    return
  }
  try {
    if (editing.value) await categoriesApi.update(editing.value.categoryNumber, name.value.trim())
    else await categoriesApi.create(name.value.trim())
    toast.add({ severity: 'success', summary: 'Збережено', life: 2000 })
    dialogVisible.value = false
    await load()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося зберегти'), life: 4000 })
  }
}

function confirmDelete(c: Category) {
  confirm.require({
    message: `Видалити категорію «${c.categoryName}»?`,
    header: 'Підтвердження',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Видалити',
    rejectLabel: 'Скасувати',
    acceptProps: { severity: 'danger' },
    accept: async () => {
      try {
        await categoriesApi.remove(c.categoryNumber)
        toast.add({ severity: 'success', summary: 'Видалено', life: 2000 })
        await load()
      } catch (e) {
        toast.add({ severity: 'error', summary: 'Помилка', detail: describeError(e, 'Не вдалося видалити'), life: 4000 })
      }
    },
  })
}

function printAll() {
  printReport('Звіт: категорії', [
    { header: '№', value: (c: Category) => c.categoryNumber },
    { header: 'Назва', value: (c: Category) => c.categoryName },
  ], categories.value)
}

onMounted(load)
</script>

<template>
  <div class="page">
    <div class="page-header">
      <h1>Категорії</h1>
      <div class="toolbar">
        <Button label="Друк" icon="pi pi-print" severity="secondary" outlined @click="printAll" />
        <Button label="Додати" icon="pi pi-plus" @click="openCreate" />
      </div>
    </div>

    <DataTable :value="categories" :loading="loading" paginator :rows="10" striped-rows data-key="categoryNumber"
      sort-field="categoryName" :sort-order="1">
      <Column field="categoryNumber" header="№" sortable :style="{ width: '6rem' }" />
      <Column field="categoryName" header="Назва" sortable />
      <Column header="" :style="{ width: '7rem' }">
        <template #body="{ data }">
          <Button icon="pi pi-pencil" text rounded @click="openEdit(data)" />
          <Button icon="pi pi-trash" text rounded severity="danger" @click="confirmDelete(data)" />
        </template>
      </Column>
    </DataTable>

    <Dialog v-model:visible="dialogVisible" :header="editing ? 'Редагувати категорію' : 'Нова категорія'" modal
      :style="{ width: '400px' }">
      <div class="field">
        <label>Назва</label>
        <InputText v-model="name" fluid @keyup.enter="save" />
      </div>
      <template #footer>
        <Button label="Скасувати" severity="secondary" text @click="dialogVisible = false" />
        <Button label="Зберегти" icon="pi pi-check" @click="save" />
      </template>
    </Dialog>
  </div>
</template>

<style scoped>
.field {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
}
</style>
