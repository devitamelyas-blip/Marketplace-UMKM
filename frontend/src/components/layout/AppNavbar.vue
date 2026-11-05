<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { useCartStore } from '@/stores/cart.store'

const router = useRouter()
const authStore = useAuthStore()
const cartStore = useCartStore()

const isMenuOpen = ref(false)

async function handleLogout() {
  await authStore.logout()
  router.push('/login')
}
</script>

<template>
  <header class="sticky top-0 z-40 bg-white/95 backdrop-blur-md border-b border-gray-100">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-16">
        <!-- Brand Logo -->
        <router-link to="/" class="flex items-center gap-2.5">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-tr from-brand-600 to-amber-500 flex items-center justify-center text-white font-black text-xl shadow-sm">
            UJ
          </div>
          <div>
            <span class="font-extrabold text-lg text-gray-900 tracking-tight">Pasar UMKM</span>
            <span class="block text-[11px] font-semibold text-brand-600 -mt-1 tracking-wider uppercase">Jawa Timur</span>
          </div>
        </router-link>

        <!-- Navigation Links -->
        <nav class="hidden md:flex items-center gap-6 text-sm font-semibold text-gray-600">
          <router-link to="/" class="hover:text-brand-600 transition-colors">Katalog Produk</router-link>
          <router-link v-if="authStore.isSeller" to="/seller/dashboard" class="hover:text-brand-600 transition-colors">Dashboard Toko</router-link>
          <router-link v-if="authStore.isAdmin" to="/admin/dashboard" class="hover:text-brand-600 transition-colors">Admin Panel</router-link>
          <router-link v-if="authStore.isAuthenticated" to="/pesanan" class="hover:text-brand-600 transition-colors">Pesanan Saya</router-link>
        </nav>

        <!-- Right Action Items -->
        <div class="flex items-center gap-4">
          <!-- Cart Icon -->
          <router-link to="/keranjang" class="relative p-2 text-gray-600 hover:text-brand-600 transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
            <span v-if="cartStore.totalCount > 0" class="absolute top-1 right-1 w-5 h-5 bg-brand-600 text-white text-[11px] font-bold rounded-full flex items-center justify-center">
              {{ cartStore.totalCount }}
            </span>
          </router-link>

          <!-- User Session -->
          <div v-if="authStore.isAuthenticated" class="flex items-center gap-3">
            <span class="hidden sm:inline-block text-xs font-semibold px-2 py-1 rounded-md bg-gray-100 text-gray-700">
              {{ authStore.user?.fullName }} ({{ authStore.user?.role }})
            </span>
            <button @click="handleLogout" class="text-xs font-semibold text-rose-600 hover:text-rose-700 bg-rose-50 px-3 py-1.5 rounded-lg border border-rose-100">
              Keluar
            </button>
          </div>
          <div v-else class="flex items-center gap-2">
            <router-link to="/login" class="text-sm font-semibold text-gray-700 hover:text-brand-600 px-3 py-2">
              Masuk
            </router-link>
            <router-link to="/daftar" class="btn-primary text-xs py-2">
              Daftar
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </header>
</template>
