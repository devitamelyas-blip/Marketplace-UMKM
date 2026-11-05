<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart.store'
import { useUiStore } from '@/stores/ui.store'
import { formatCurrency } from '@/utils/format'

const router = useRouter()
const cartStore = useCartStore()
const uiStore = useUiStore()

const recipientName = ref('Budi Santoso')
const phoneNumber = ref('081234567890')
const street = ref('Jl. Raya Darmo No. 45, Wonokromo')
const city = ref('Surabaya')
const selectedCourier = ref('JNE')
const selectedService = ref('REG')
const shippingCost = ref(15000)

const isProcessing = ref(false)

function handlePay() {
  isProcessing.value = true
  setTimeout(() => {
    isProcessing.value = false
    uiStore.showToast('success', 'Pesanan berhasil dibuat! Simulasi Snap popup Midtrans terbuka.')
    cartStore.clearCart()
    router.push('/pesanan')
  }, 1200)
}
</script>

<template>
  <div class="max-w-4xl mx-auto space-y-8">
    <h1 class="text-2xl font-black text-gray-900">Checkout & Pembayaran</h1>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Checkout Form -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Alamat Pengiriman -->
        <div class="bg-white p-6 rounded-2xl border border-gray-100 shadow-sm space-y-4">
          <h2 class="font-bold text-base text-gray-900 flex items-center gap-2">
            <span>📍</span> Alamat Pengiriman
          </h2>
          <div class="space-y-3 text-sm">
            <div>
              <label class="block text-xs font-semibold text-gray-700 mb-1">Nama Penerima</label>
              <input v-model="recipientName" class="w-full px-3 py-2 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500" />
            </div>
            <div>
              <label class="block text-xs font-semibold text-gray-700 mb-1">Nomor Telepon</label>
              <input v-model="phoneNumber" class="w-full px-3 py-2 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500" />
            </div>
            <div>
              <label class="block text-xs font-semibold text-gray-700 mb-1">Alamat Lengkap</label>
              <textarea v-model="street" rows="2" class="w-full px-3 py-2 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500"></textarea>
            </div>
            <div>
              <label class="block text-xs font-semibold text-gray-700 mb-1">Kota / Kabupaten</label>
              <input v-model="city" class="w-full px-3 py-2 rounded-xl border border-gray-200 text-sm focus:outline-none focus:border-brand-500" />
            </div>
          </div>
        </div>

        <!-- Kurir Pengiriman -->
        <div class="bg-white p-6 rounded-2xl border border-gray-100 shadow-sm space-y-4">
          <h2 class="font-bold text-base text-gray-900 flex items-center gap-2">
            <span>🚚</span> Opsi Pengiriman (Mock RajaOngkir)
          </h2>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <label class="flex items-center justify-between p-3.5 rounded-xl border-2 cursor-pointer transition-all"
              :class="selectedCourier === 'JNE' ? 'border-brand-600 bg-brand-50/50' : 'border-gray-100'">
              <div class="flex items-center gap-2.5">
                <input type="radio" value="JNE" v-model="selectedCourier" class="text-brand-600" />
                <div>
                  <span class="font-bold text-xs block text-gray-900">JNE Layanan REG</span>
                  <span class="text-[11px] text-gray-500">Estimasi 1-2 Hari</span>
                </div>
              </div>
              <span class="font-extrabold text-xs text-brand-600">{{ formatCurrency(15000) }}</span>
            </label>

            <label class="flex items-center justify-between p-3.5 rounded-xl border-2 cursor-pointer transition-all"
              :class="selectedCourier === 'SiCepat' ? 'border-brand-600 bg-brand-50/50' : 'border-gray-100'">
              <div class="flex items-center gap-2.5">
                <input type="radio" value="SiCepat" v-model="selectedCourier" class="text-brand-600" />
                <div>
                  <span class="font-bold text-xs block text-gray-900">SiCepat REG</span>
                  <span class="text-[11px] text-gray-500">Estimasi 1-2 Hari</span>
                </div>
              </div>
              <span class="font-extrabold text-xs text-brand-600">{{ formatCurrency(16000) }}</span>
            </label>
          </div>
        </div>
      </div>

      <!-- Ringkasan & Bayar -->
      <div class="space-y-4">
        <div class="bg-white p-6 rounded-2xl border border-gray-100 shadow-sm space-y-4">
          <h3 class="font-bold text-base text-gray-900">Total Pembayaran</h3>

          <div class="space-y-2 text-xs">
            <div class="flex justify-between text-gray-600">
              <span>Subtotal Produk</span>
              <span class="font-semibold">{{ formatCurrency(cartStore.totalPrice) }}</span>
            </div>
            <div class="flex justify-between text-gray-600">
              <span>Ongkos Kirim</span>
              <span class="font-semibold">{{ formatCurrency(shippingCost) }}</span>
            </div>
          </div>

          <div class="border-t border-gray-100 pt-3 flex justify-between items-center">
            <span class="font-bold text-sm text-gray-900">Total Tagihan</span>
            <span class="font-extrabold text-lg text-brand-600">{{ formatCurrency(cartStore.totalPrice + shippingCost) }}</span>
          </div>

          <button
            @click="handlePay"
            :disabled="isProcessing || cartStore.items.length === 0"
            class="btn-primary w-full text-sm py-3"
          >
            {{ isProcessing ? 'Memproses Midtrans Snap...' : 'Bayar via Midtrans' }}
          </button>
        </div>

        <div class="bg-amber-50 p-4 rounded-xl border border-amber-200 text-xs text-amber-800 leading-relaxed">
          <span class="font-bold block mb-1">🛡️ Catatan Keamanan Produksi:</span>
          Saat tombol bayar ditekan, backend menggunakan EF Core <strong>Optimistic Concurrency</strong> (RowVersion) untuk memverifikasi stok saat race condition terjadi.
        </div>
      </div>
    </div>
  </div>
</template>
