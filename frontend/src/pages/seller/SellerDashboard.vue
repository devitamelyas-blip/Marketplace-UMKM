<script setup lang="ts">
import { ref } from 'vue'

const stats = ref([
  { label: 'Total Produk Aktif', value: '6 Produk', change: '+2 bulan ini' },
  { label: 'Pesanan Masuk', value: '18 Pesanan', change: '4 perlu dikirim' },
  { label: 'Total Penjualan', value: 'Rp 2.850.000', change: '+14% minggu ini' },
  { label: 'Rating Rata-rata', value: '4.9 / 5.0', change: '24 ulasan' }
])
</script>

<template>
  <div class="space-y-8">
    <div>
      <h1 class="text-2xl font-black text-gray-900">Dashboard Seller</h1>
      <p class="text-xs text-gray-500">Kelola inventori produk UMKM dan pantau pesanan pelanggan.</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div v-for="stat in stats" :key="stat.label" class="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm space-y-1">
        <span class="text-xs font-semibold text-gray-400 uppercase tracking-wider">{{ stat.label }}</span>
        <div class="text-2xl font-extrabold text-gray-900">{{ stat.value }}</div>
        <span class="text-xs font-medium text-emerald-600 block">{{ stat.change }}</span>
      </div>
    </div>
  </div>
</template>
