<script setup lang="ts">
import { computed } from 'vue'
import { RouterView, useRouter } from 'vue-router'
import Menubar from 'primevue/menubar'
import Button from 'primevue/button'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

interface NavItem {
  label: string
  icon: string
  route: string
  roles?: Array<'Manager' | 'Cachier'>
}

const allItems: NavItem[] = [
  { label: 'Головна', icon: 'pi pi-home', route: '/' },
  { label: 'Працівники', icon: 'pi pi-users', route: '/employees', roles: ['Manager'] },
  { label: 'Товари', icon: 'pi pi-box', route: '/products' },
  { label: 'Товари в магазині', icon: 'pi pi-shopping-cart', route: '/store-products' },
  { label: 'Категорії', icon: 'pi pi-tags', route: '/categories', roles: ['Manager'] },
  { label: 'Карти клієнтів', icon: 'pi pi-id-card', route: '/customer-cards' },
  { label: 'Чеки', icon: 'pi pi-receipt', route: '/checks' },
  { label: 'Статистика', icon: 'pi pi-chart-bar', route: '/statistics', roles: ['Manager'] },
]

const menuItems = computed(() =>
  allItems
    .filter((i) => !i.roles || (auth.role && i.roles.includes(auth.role)))
    .map((i) => ({
      label: i.label,
      icon: i.icon,
      command: () => router.push(i.route),
    })),
)

function logout() {
  auth.logout()
  router.push({ name: 'login' })
}
</script>

<template>
  <div class="layout">
    <Menubar :model="menuItems" class="app-menubar">
      <template #start>
        <span class="brand">ZLAGODA</span>
      </template>
      <template #end>
        <div class="user-area">
          <span class="user-info">
            <i class="pi pi-user" />
            {{ auth.username }} · {{ auth.role === 'Manager' ? 'Менеджер' : 'Касир' }}
          </span>
          <Button
            label="Вийти"
            icon="pi pi-sign-out"
            severity="secondary"
            text
            @click="logout"
          />
        </div>
      </template>
    </Menubar>

    <main>
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
.brand {
  font-weight: 700;
  font-size: 1.25rem;
  letter-spacing: 0.05em;
  margin-right: 1rem;
  color: var(--p-primary-color);
}

.app-menubar {
  border-radius: 0;
  border-left: none;
  border-right: none;
  border-top: none;
}

.user-area {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.user-info {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.9rem;
  color: #4b5563;
}
</style>
