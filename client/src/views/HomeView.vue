<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import Card from 'primevue/card'

const auth = useAuthStore()
</script>

<template>
  <div class="page">
    <div class="page-header">
      <h1>Вітаємо, {{ auth.username }}</h1>
    </div>

    <Card>
      <template #content>
        <p>
          Ви увійшли як
          <strong>{{ auth.role === 'Manager' ? 'менеджер' : 'касир' }}</strong>.
        </p>
        <p v-if="auth.isManager">
          Вам доступне керування працівниками, товарами, категоріями, картами клієнтів
          та чеками.
        </p>
        <p v-else>
          Вам доступний перегляд товарів, карт клієнтів і створення чеків.
        </p>
      </template>
    </Card>
  </div>
</template>
