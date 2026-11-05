<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart.store'
import { formatCurrency } from '@/utils/format'

const router = useRouter()
const cartStore = useCartStore()

function proceedToCheckout() {
  router.push('/checkout')
}
</script>

<template>
  <div class="max-w-4xl mx-auto space-y-6">
    <h1 class="text-2xl font-black text-gray-900">Keranjang Belanja</h1>

    <div v-if="cartStore.items.length > 0" class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Item List -->
      <div class="lg:col-span-2 space-y-4">
        <div
          v-for="item in cartStore.items"
          :key="item.productId"
          class="bg-white p-4 rounded-2xl border border-gray-100 shadow-sm flex gap-4 items-center"
        >
          <div class="w-20 h-20 bg-amber-50 rounded-xl flex items-center justify-center font-bold text-xs text-brand-600 p-2 text-center">
            {{ item.name }}
          </div>

          <div class="flex-1">
            <span class="text-[11px] font-semibold text-gray-400 uppercase tracking-wider">{{ item.originCity }} &bull; {{ item.shopName }}</span>
            <h3 class="font-bold text-sm text-gray-900 line-clamp-1">{{ item.name }}</h3>
            <span class="font-extrabold text-sm text-brand-600 block mt-1">{{ formatCurrency(item.price) }}</span>

            <div class="flex items-center gap-3 mt-3">
              <div class="flex items-center border border-gray-200 rounded-lg overflow-hidden">
                <button @click="cartStore.updateQuantity(item.productId, item.quantity - 1)" class="px-2.5 py-1 text-gray-600 hover:bg-gray-100 font-bold">-</button>
                <span class="px-3 py-1 text-xs font-bold text-gray-800">{{ item.quantity }}</span>
                <button @click="cartStore.updateQuantity(item.productId, item.quantity + 1)" class="px-2.5 py-1 text-gray-600 hover:bg-gray-100 font-bold">+</button>
              </div>

              <button @click="cartStore.removeFromCart(item.productId)" class="text-xs font-semibold text-rose-600 hover:text-rose-700">
                Hapus
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Order Summary -->
      <div class="bg-white p-6 rounded-2xl border border-gray-100 shadow-sm space-y-4 h-fit">
        <h3 class="font-bold text-base text-gray-900">Ringkasan Belanja</h3>

        <div class="space-y-2 text-xs">
          <div class="flex justify-between text-gray-600">
            <span>Total Item</span>
            <span class="font-semibold">{{ cartStore.totalCount }} barang</span>
          </div>
          <div class="flex justify-between text-gray-600">
            <span>Subtotal Produk</span>
            <span class="font-semibold">{{ formatCurrency(cartStore.totalPrice) }}</span>
          </div>
        </div>

        <div class="border-t border-gray-100 pt-3 flex justify-between items-center">
          <span class="font-bold text-sm text-gray-900">Total Harga</span>
          <span class="font-extrabold text-lg text-brand-600">{{ formatCurrency(cartStore.totalPrice) }}</span>
        </div>

        <button @click="proceedToCheckout" class="btn-primary w-full text-sm py-3">
          Lanjut ke Pembayaran
        </button>
      </div>
    </div>

    <div v-else class="text-center py-20 bg-white rounded-3xl border border-gray-100 space-y-4">
      <div class="w-16 h-16 bg-gray-100 text-gray-400 rounded-full flex items-center justify-center mx-auto text-2xl font-bold">
        🛒
      </div>
      <h3 class="font-bold text-gray-900 text-lg">Keranjang Belanja Masih Kosong</h3>
      <p class="text-sm text-gray-500 max-w-sm mx-auto">
        Temukan aneka oleh-oleh, kerajinan, dan kuliner khas Jawa Timur sekarang juga.
      </p>
      <router-link to="/" class="btn-primary text-xs inline-block">
        Mulai Belanja
      </router-link>
    </div>
  </div>
</template>
