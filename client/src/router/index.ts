import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type { EmployeeRole } from '@/types'

declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    roles?: EmployeeRole[]
    title?: string
  }
}

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/LoginView.vue'),
    meta: { title: 'Вхід' },
  },
  {
    path: '/',
    component: () => import('@/layouts/MainLayout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'home',
        component: () => import('@/views/HomeView.vue'),
        meta: { title: 'Головна' },
      },
      {
        path: 'employees',
        name: 'employees',
        component: () => import('@/views/employees/EmployeeListView.vue'),
        meta: { roles: ['Manager'], title: 'Працівники' },
      },
      {
        path: 'products',
        name: 'products',
        component: () => import('@/views/products/ProductListView.vue'),
        meta: { title: 'Товари' },
      },
      {
        path: 'store-products',
        name: 'store-products',
        component: () => import('@/views/storeProducts/StoreProductListView.vue'),
        meta: { title: 'Товари в магазині' },
      },
      {
        path: 'categories',
        name: 'categories',
        component: () => import('@/views/categories/CategoryListView.vue'),
        meta: { roles: ['Manager'], title: 'Категорії' },
      },
      {
        path: 'customer-cards',
        name: 'customer-cards',
        component: () => import('@/views/customerCards/CustomerCardListView.vue'),
        meta: { title: 'Карти клієнтів' },
      },
      {
        path: 'checks',
        name: 'checks',
        component: () => import('@/views/checks/CheckListView.vue'),
        meta: { title: 'Чеки' },
      },
    ],
  },
  { path: '/:pathMatch(.*)*', redirect: '/' },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to) => {
  const auth = useAuthStore()

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (to.name === 'login' && auth.isAuthenticated) {
    return { name: 'home' }
  }

  if (to.meta.roles && auth.role && !to.meta.roles.includes(auth.role)) {
    return { name: 'home' }
  }

  return true
})

export default router
