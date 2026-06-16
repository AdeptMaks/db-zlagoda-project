<script setup lang="ts">
import { reactive, ref, watch } from 'vue'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import Password from 'primevue/password'
import DatePicker from 'primevue/datepicker'
import Select from 'primevue/select'
import Button from 'primevue/button'
import Message from 'primevue/message'
import type {
  CreateEmployeeRequest,
  Employee,
  EmployeeRole,
  UpdateEmployeeRequest,
} from '@/types'

const props = defineProps<{
  visible: boolean
  employee: Employee | null
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  'save-create': [body: CreateEmployeeRequest]
  'save-update': [id: string, body: UpdateEmployeeRequest]
}>()

const roleOptions: Array<{ label: string; value: EmployeeRole }> = [
  { label: 'Менеджер', value: 'Manager' },
  { label: 'Касир', value: 'Cachier' },
]

interface FormState {
  employeeRole: EmployeeRole
  surname: string
  firstname: string
  patronymic: string
  username: string
  password: string
  salary: number | null
  birthDate: Date | null
  startDate: Date | null
  phoneNumber: string
  city: string
  street: string
  zipCode: string
}

function emptyForm(): FormState {
  return {
    employeeRole: 'Cachier',
    surname: '',
    firstname: '',
    patronymic: '',
    username: '',
    password: '',
    salary: null,
    birthDate: null,
    startDate: null,
    phoneNumber: '',
    city: '',
    street: '',
    zipCode: '',
  }
}

const form = reactive<FormState>(emptyForm())
const errors = ref<string[]>([])

const isEdit = () => props.employee !== null

watch(
  () => props.visible,
  (open) => {
    if (!open) return
    errors.value = []
    const e = props.employee
    if (e) {
      Object.assign(form, {
        employeeRole: e.employeeRole,
        surname: e.surname,
        firstname: e.firstname,
        patronymic: e.patronymic ?? '',
        username: '',
        password: '',
        salary: e.salary,
        birthDate: e.birthDate ? new Date(e.birthDate) : null,
        startDate: e.startDate ? new Date(e.startDate) : null,
        phoneNumber: e.phoneNumber,
        city: e.city,
        street: e.street,
        zipCode: e.zipCode,
      })
    } else {
      Object.assign(form, emptyForm())
    }
  },
)

function ageAtLeast18(birth: Date): boolean {
  const cutoff = new Date()
  cutoff.setFullYear(cutoff.getFullYear() - 18)
  return birth <= cutoff
}

function validate(): boolean {
  const errs: string[] = []
  if (!form.surname.trim()) errs.push('Прізвище обовʼязкове.')
  if (!form.firstname.trim()) errs.push("Імʼя обовʼязкове.")
  if (!isEdit()) {
    if (form.username.trim().length < 10 || form.username.trim().length > 30)
      errs.push('Логін має містити від 10 до 30 символів.')
    if (form.password.length < 10 || form.password.length > 20)
      errs.push('Пароль має містити від 10 до 20 символів.')
  }
  if (form.salary === null || form.salary < 0) errs.push('Зарплата не може бути відʼємною.')
  if (!form.birthDate) errs.push('Вкажіть дату народження.')
  else if (!ageAtLeast18(form.birthDate)) errs.push('Вік працівника має бути не менше 18 років.')
  if (!form.startDate) errs.push('Вкажіть дату початку роботи.')
  if (form.phoneNumber.trim().length !== 13)
    errs.push('Телефон має містити рівно 13 символів (разом із «+»).')
  if (!form.city.trim() || !form.street.trim())
    errs.push('Заповніть адресу (місто, вулиця).')
  if (form.zipCode.trim().length !== 9)
    errs.push('Індекс має містити рівно 9 символів.')

  errors.value = errs
  return errs.length === 0
}

function toIsoDate(d: Date): string {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(
    d.getDate(),
  ).padStart(2, '0')}`
}

function submit() {
  if (!validate()) return

  const address = { city: form.city, street: form.street, zipCode: form.zipCode }

  if (isEdit()) {
    const body: UpdateEmployeeRequest = {
      employeeRole: form.employeeRole,
      surname: form.surname,
      firstname: form.firstname,
      patronymic: form.patronymic || null,
      salary: form.salary!,
      birthDate: toIsoDate(form.birthDate!),
      startDate: toIsoDate(form.startDate!),
      phoneNumber: form.phoneNumber,
      addressInfo: address,
    }
    emit('save-update', props.employee!.employeeId, body)
  } else {
    const body: CreateEmployeeRequest = {
      employeeRole: form.employeeRole,
      surname: form.surname,
      firstname: form.firstname,
      username: form.username,
      password: form.password,
      patronymic: form.patronymic || null,
      salary: form.salary!,
      birthDate: toIsoDate(form.birthDate!),
      startDate: toIsoDate(form.startDate!),
      phoneNumber: form.phoneNumber,
      addressInfo: address,
    }
    emit('save-create', body)
  }
}
</script>

<template>
  <Dialog
    :visible="visible"
    :header="isEdit() ? 'Редагувати працівника' : 'Новий працівник'"
    modal
    :style="{ width: '560px' }"
    @update:visible="emit('update:visible', $event)"
  >
    <div class="form-grid">
      <label>
        <span>Роль</span>
        <Select v-model="form.employeeRole" :options="roleOptions" option-label="label" option-value="value" fluid />
      </label>
      <label>
        <span>Зарплата</span>
        <InputNumber v-model="form.salary" mode="decimal" :min-fraction-digits="2" :min="0" fluid />
      </label>

      <label>
        <span>Прізвище</span>
        <InputText v-model="form.surname" fluid />
      </label>
      <label>
        <span>Імʼя</span>
        <InputText v-model="form.firstname" fluid />
      </label>
      <label class="span-2">
        <span>По батькові</span>
        <InputText v-model="form.patronymic" fluid />
      </label>

      <template v-if="!isEdit()">
        <label>
          <span>Логін</span>
          <InputText v-model="form.username" autocomplete="off" fluid />
        </label>
        <label>
          <span>Пароль</span>
          <Password v-model="form.password" :feedback="false" toggle-mask fluid />
        </label>
      </template>

      <label>
        <span>Дата народження</span>
        <DatePicker v-model="form.birthDate" date-format="yy-mm-dd" show-icon fluid />
      </label>
      <label>
        <span>Дата початку роботи</span>
        <DatePicker v-model="form.startDate" date-format="yy-mm-dd" show-icon fluid />
      </label>

      <label class="span-2">
        <span>Телефон</span>
        <InputText v-model="form.phoneNumber" placeholder="+380..." fluid />
      </label>

      <label>
        <span>Місто</span>
        <InputText v-model="form.city" fluid />
      </label>
      <label>
        <span>Вулиця</span>
        <InputText v-model="form.street" fluid />
      </label>
      <label>
        <span>Індекс</span>
        <InputText v-model="form.zipCode" fluid />
      </label>
    </div>

    <Message v-if="errors.length" severity="error" :closable="false" class="form-errors">
      <ul>
        <li v-for="(e, i) in errors" :key="i">{{ e }}</li>
      </ul>
    </Message>

    <template #footer>
      <Button label="Скасувати" severity="secondary" text @click="emit('update:visible', false)" />
      <Button label="Зберегти" icon="pi pi-check" @click="submit" />
    </template>
  </Dialog>
</template>

<style scoped>
.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.9rem;
}

.form-grid label {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
  font-size: 0.85rem;
}

.span-2 {
  grid-column: 1 / -1;
}

.form-errors ul {
  margin: 0;
  padding-left: 1.1rem;
}
</style>
