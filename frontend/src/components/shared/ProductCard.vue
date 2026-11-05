<script setup lang="ts">
import type { Product } from '@/types'
import { formatCurrency } from '@/utils/format'
import { useCartStore } from '@/stores/cart.store'
import { useUiStore } from '@/stores/ui.store'

const props = defineProps<{
  product: Product
}>()

const cartStore = useCartStore()
const uiStore = useUiStore()

function handleAddToCart() {
  if (props.product.stock <= 0) return
  cartStore.addToCart(props.product, 1)
  uiStore.showToast('success', `${props.product.name} dimasukkan ke keranjang!`)
}
</script>

<template>
  <div class="group bg-white rounded-2xl border border-gray-100 hover:border-brand-200 shadow-sm hover:shadow-xl transition-all duration-300 flex flex-col overflow-hidden">
    <!-- Image & Badge Container -->
    <div class="relative aspect-square bg-gray-100 overflow-hidden">
      <div class="w-full h-full flex items-center justify-center bg-gradient-to-br from-amber-50 to-orange-100 text-brand-600 font-bold text-sm">
        {{ product.name }}
      </div>

      <!-- Badge Asli Jatim -->
      <div v-if="product.isJatimBadge" class="absolute top-2.5 left-2.5 badge-jatim shadow-sm">
        <svg class="w-3.5 h-3.5 text-amber-600" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M6.267 3.455a3.066 3.066 0 001.745-.723 3.066 3.066 0 013.976 0 3.066 3.066 0 001.745.723 3.066 3.066 0 012.812 2.812c.051.643.304 1.254.723 1.745a3.066 3.066 0 010 3.976 3.066 3.066 0 00-.723 1.745 3.066 3.066 0 01-2.812 2.812 3.066 3.066 0 00-1.745.723 3.066 3.066 0 01-3.976 0 3.066 3.066 0 00-1.745-.723 3.066 3.066 0 01-2.812-2.812 3.066 3.066 0 00-.723-1.745 3.066 3.066 0 010-3.976 3.066 3.066 0 00.723-1.745 3.066 3.066 0 012.812-2.812zm7.44 5.252a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
        </svg>
        <span>Asli Jatim</span>
      </div>

      <!-- Out of Stock Overlay -->
      <div v-if="product.stock <= 0" class="absolute inset-0 bg-black/60 backdrop-blur-[2px] flex items-center justify-center">
        <span class="px-3 py-1 bg-rose-600 text-white text-xs font-bold rounded-lg uppercase tracking-wider">Stok Habis</span>
      </div>
    </div>

    <!-- Product Info -->
    <div class="p-4 flex-1 flex flex-col justify-between">
      <div>
        <div class="text-[11px] font-semibold text-gray-500 uppercase tracking-wider mb-1 flex items-center gap-1">
          <svg class="w-3 h-3 text-brand-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
          </svg>
          {{ product.originCity }}
        </div>
        <h3 class="font-bold text-gray-900 text-sm line-clamp-2 mb-2 group-hover:text-brand-600 transition-colors">
          {{ product.name }}
        </h3>
      </div>

      <div class="mt-4 pt-3 border-t border-gray-100 flex items-center justify-between">
        <div>
          <span class="text-xs text-gray-400 block -mb-0.5">Harga</span>
          <span class="font-extrabold text-base text-gray-900">{{ formatCurrency(product.price) }}</span>
        </div>

        <button
          @click="handleAddToCart"
          :disabled="product.stock <= 0"
          class="p-2.5 rounded-xl bg-brand-50 hover:bg-brand-600 text-brand-600 hover:text-white transition-colors disabled:opacity-40 disabled:pointer-events-none"
          title="Tambah ke Keranjang"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
        </button>
      </div>
    </div>
  </div>
</template>
