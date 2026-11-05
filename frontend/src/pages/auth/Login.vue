<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { useUiStore } from '@/stores/ui.store'

const router = useRouter()
const authStore = useAuthStore()
const uiStore = useUiStore()

const email = ref('admin@umkmjatim.id')
const password = ref('Admin@12345')
const isLoading = ref(false)

async function handleLogin() {
  isLoading.value = true
  try {
    await authStore.login({ email: email.value, password: password.value })
    uiStore.showToast('success', 'Berhasil masuk!')
    router.push('/')
  } catch {
    uiStore.showToast('error', 'Login gagal. Periksa kembali email dan password.')
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="min-h-[70vh] flex items-center justify-center py-12 px-4">
    <div class="max-w-md w-full bg-white p-8 rounded-3xl border border-gray-100 shadow-xl space-y-6">
      <div class="text-center space-y-1">
        <h2 class="text-2xl font-black text-gray-900 tracking-tight">Selamat Datang Kembali</h2>
        <p class="text-xs text-gray-500">Masuk ke akun Pasar UMKM Jawa Timur Anda</p>
      </div>

      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
          <label class="block text-xs font-bold text-gray-700 mb-1">Email</label>
          <input
            v-model="email"
            type="email"
            required
            class="w-full px-3.5 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500"
          />
        </div>

        <div>
          <label class="block text-xs font-bold text-gray-700 mb-1">Password</label>
          <input
            v-model="password"
            type="password"
            required
            class="w-full px-3.5 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500"
          />
        </div>

        <button
          type="submit"
          :disabled="isLoading"
          class="btn-primary w-full text-sm py-3 mt-2"
        >
          {{ isLoading ? 'Memverifikasi...' : 'Masuk Sekarang' }}
        </button>
      </form>

      <!-- Quick Demo Account Pills -->
      <div class="pt-4 border-t border-gray-100 text-xs text-gray-600 space-y-2">
        <span class="font-bold text-[11px] uppercase tracking-wider text-gray-400 block">Akun Demo Cepat:</span>
        <div class="flex flex-wrap gap-1.5">
          <button @click="email = 'admin@umkmjatim.id'; password = 'Admin@12345'" type="button" class="px-2.5 py-1 rounded-lg bg-gray-100 hover:bg-gray-200 text-[11px] font-semibold">
            Admin
          </button>
          <button @click="email = 'toko.surabaya@umkmjatim.id'; password = 'Seller@12345'" type="button" class="px-2.5 py-1 rounded-lg bg-gray-100 hover:bg-gray-200 text-[11px] font-semibold">
            Seller Sby
          </button>
          <button @click="email = 'customer1@test.id'; password = 'Customer@12345'" type="button" class="px-2.5 py-1 rounded-lg bg-gray-100 hover:bg-gray-200 text-[11px] font-semibold">
            Customer
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
