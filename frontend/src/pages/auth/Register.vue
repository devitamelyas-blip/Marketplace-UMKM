<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { useUiStore } from '@/stores/ui.store'

const router = useRouter()
const authStore = useAuthStore()
const uiStore = useUiStore()

const fullName = ref('')
const email = ref('')
const phoneNumber = ref('')
const password = ref('')
const confirmPassword = ref('')
const isLoading = ref(false)

async function handleRegister() {
  if (password.value !== confirmPassword.value) {
    uiStore.showToast('error', 'Konfirmasi password tidak cocok!')
    return
  }

  isLoading.value = true
  try {
    await authStore.register({
      fullName: fullName.value,
      email: email.value,
      phoneNumber: phoneNumber.value,
      password: password.value,
      confirmPassword: confirmPassword.value
    })
    uiStore.showToast('success', 'Pendaftaran akun berhasil! Silakan login.')
    router.push('/login')
  } catch {
    uiStore.showToast('error', 'Pendaftaran gagal. Pastikan format data valid.')
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="min-h-[70vh] flex items-center justify-center py-12 px-4">
    <div class="max-w-md w-full bg-white p-8 rounded-3xl border border-gray-100 shadow-xl space-y-6">
      <div class="text-center space-y-1">
        <h2 class="text-2xl font-black text-gray-900 tracking-tight">Daftar Akun Baru</h2>
        <p class="text-xs text-gray-500">Mulai belanja produk UMKM lokal khas Jawa Timur</p>
      </div>

      <form @submit.prevent="handleRegister" class="space-y-3 text-sm">
        <div>
          <label class="block text-xs font-bold text-gray-700 mb-1">Nama Lengkap</label>
          <input v-model="fullName" required class="w-full px-3.5 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500" />
        </div>
        <div>
          <label class="block text-xs font-bold text-gray-700 mb-1">Email</label>
          <input v-model="email" type="email" required class="w-full px-3.5 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500" />
        </div>
        <div>
          <label class="block text-xs font-bold text-gray-700 mb-1">Nomor WhatsApp / HP</label>
          <input v-model="phoneNumber" required class="w-full px-3.5 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500" />
        </div>
        <div>
          <label class="block text-xs font-bold text-gray-700 mb-1">Password</label>
          <input v-model="password" type="password" required minlength="8" class="w-full px-3.5 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500" />
        </div>
        <div>
          <label class="block text-xs font-bold text-gray-700 mb-1">Konfirmasi Password</label>
          <input v-model="confirmPassword" type="password" required minlength="8" class="w-full px-3.5 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500" />
        </div>

        <button type="submit" :disabled="isLoading" class="btn-primary w-full text-sm py-3 mt-4">
          {{ isLoading ? 'Mendaftarkan...' : 'Daftar Sekarang' }}
        </button>
      </form>
    </div>
  </div>
</template>
