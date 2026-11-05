<script setup lang="ts">
import { ref } from 'vue'

const stats = ref([
  { label: 'Total Transaksi', value: 'Rp 14.820.000' },
  { label: 'Total Seller UMKM', value: '5 Toko Aktif' },
  { label: 'Total Produk Terdaftar', value: '30 Produk' },
  { label: 'Badge "Asli Jatim"', value: '10 Terverifikasi' }
])
</script>

<template>
  <div class="space-y-8">
    <div>
      <h1 class="text-2xl font-black text-gray-900">Panel Administrator</h1>
      <p class="text-xs text-gray-500">Pusat kendali marketplace, verifikasi badge keaslian daerah, dan audit log transaksi.</p>
    </div>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div v-for="stat in stats" :key="stat.label" class="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm space-y-1">
        <span class="text-xs font-semibold text-gray-400 uppercase tracking-wider">{{ stat.label }}</span>
        <div class="text-2xl font-extrabold text-gray-900">{{ stat.value }}</div>
      </div>
    </div>
  </div>
</template>
