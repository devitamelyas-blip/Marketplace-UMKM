<script setup lang="ts">
import { ref, computed } from 'vue'
import ProductCard from '@/components/shared/ProductCard.vue'
import type { Product } from '@/types'

// Demo products realistis sesuai seeder
const products = ref<Product[]>([
  { id: 1, shopId: 1, categoryId: 1, name: "Bumbu Rawon Khas Surabaya Ny. Giok", slug: "bumbu-rawon-khas-surabaya", description: "Bumbu rawon kluwek hitam pekat otentik resep turun-temurun tanpa bahan pengawet.", price: 38000, stock: 85, originCity: "Surabaya", originProvince: "Jawa Timur", weightGrams: 300, isActive: true, isJatimBadge: true },
  { id: 2, shopId: 1, categoryId: 1, name: "Petis Udang Super Asli Kenjeran", slug: "petis-udang-super-kenjeran", description: "Petis udang grade A warna hitam mengkilap, rasa gurih manis alami.", price: 45000, stock: 60, originCity: "Surabaya", originProvince: "Jawa Timur", weightGrams: 250, isActive: true, isJatimBadge: true },
  { id: 3, shopId: 2, categoryId: 6, name: "Keripik Apel Manalagi Malang Super Crispy", slug: "keripik-apel-manalagi-malang", description: "Keripik apel diproses vacuum frying suhu rendah aroma segar terjaga.", price: 25000, stock: 120, originCity: "Malang", originProvince: "Jawa Timur", weightGrams: 150, isActive: true, isJatimBadge: true },
  { id: 4, shopId: 2, categoryId: 2, name: "Topeng Malangan Kayu Ukir Patih Karakter Gagah", slug: "topeng-malangan-kayu-ukir", description: "Topeng kayu mahoni pahatan seniman Kedungmonggo Malang.", price: 275000, stock: 15, originCity: "Malang", originProvince: "Jawa Timur", weightGrams: 600, isActive: true, isJatimBadge: true },
  { id: 5, shopId: 3, categoryId: 3, name: "Kain Batik Tulis Sidoarjo Motif Beras Wutah", slug: "batik-tulis-sidoarjo-beras-wutah", description: "Kain katun primissima batik tulis halus motif pesisiran kampung Jetis.", price: 420000, stock: 12, originCity: "Sidoarjo", originProvince: "Jawa Timur", weightGrams: 350, isActive: true, isJatimBadge: true },
  { id: 6, shopId: 3, categoryId: 1, name: "Bandeng Asap Sidoarjo Spesial Sambal Petis", slug: "bandeng-asap-sidoarjo-spesial", description: "Bandeng tambak segar diasap menggunakan batok kelapa kering.", price: 75000, stock: 30, originCity: "Sidoarjo", originProvince: "Jawa Timur", weightGrams: 700, isActive: true, isJatimBadge: true },
  { id: 7, shopId: 4, categoryId: 2, name: "Miniatur Candi Tikus Majapahit Batu Andesit", slug: "miniatur-candi-tikus-majapahit", description: "Pahatan miniatur situs purbakala Majapahit Trowulan dari batu andesit.", price: 180000, stock: 20, originCity: "Mojokerto", originProvince: "Jawa Timur", weightGrams: 1200, isActive: true, isJatimBadge: true },
  { id: 8, shopId: 5, categoryId: 4, name: "Kopi Arabika Java Ijen Raung Single Origin (250g)", slug: "kopi-arabika-java-ijen-raung", description: "Biji kopi Arabika petik merah dari lereng vulkanik kawah Ijen beraroma melati.", price: 78000, stock: 80, originCity: "Jember", originProvince: "Jawa Timur", weightGrams: 280, isActive: true, isJatimBadge: true }
])

const searchQuery = ref('')
const selectedCity = ref('')
const selectedBadgeOnly = ref(false)

const cities = ['Surabaya', 'Malang', 'Sidoarjo', 'Mojokerto', 'Jember']

const filteredProducts = computed(() => {
  return products.value.filter(p => {
    const matchSearch = p.name.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchCity = !selectedCity.value || p.originCity === selectedCity.value
    const matchBadge = !selectedBadgeOnly.value || p.isJatimBadge
    return matchSearch && matchCity && matchBadge
  })
})
</script>

<template>
  <div class="space-y-8">
    <!-- Hero Banner -->
    <div class="relative rounded-3xl overflow-hidden bg-gradient-to-r from-brand-700 via-brand-600 to-amber-600 text-white p-8 sm:p-12 shadow-lg">
      <div class="max-w-2xl space-y-4">
        <span class="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold bg-white/20 backdrop-blur-md">
          🏛️ Ragam Karya & Cita Rasa Asli Jawa Timur
        </span>
        <h1 class="text-3xl sm:text-5xl font-black tracking-tight leading-tight">
          Beli Langsung Dari Pengrajin & UMKM Terbaik Jatim
        </h1>
        <p class="text-sm sm:text-base text-orange-100 font-normal">
          Dukung usaha lokal dengan transaksi aman, kontrol stok real-time tanpa resiko overselling, dan pembayaran online Midtrans.
        </p>
      </div>
    </div>

    <!-- Filter & Search Controls -->
    <div class="bg-white p-4 rounded-2xl border border-gray-100 shadow-sm flex flex-col md:flex-row gap-4 items-center justify-between">
      <div class="relative w-full md:w-96">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Cari rawon, batik, keripik, kopi..."
          class="w-full pl-10 pr-4 py-2.5 rounded-xl border border-gray-200 text-sm focus:border-brand-500 focus:outline-none focus:ring-1 focus:ring-brand-500"
        />
        <svg class="w-4 h-4 text-gray-400 absolute left-3.5 top-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
      </div>

      <div class="flex flex-wrap items-center gap-3 w-full md:w-auto">
        <!-- Filter Kota -->
        <select
          v-model="selectedCity"
          class="px-3 py-2.5 rounded-xl border border-gray-200 text-sm font-medium text-gray-700 focus:outline-none focus:border-brand-500 bg-white"
        >
          <option value="">Semua Kota Asal</option>
          <option v-for="city in cities" :key="city" :value="city">{{ city }}</option>
        </select>

        <!-- Toggle Badge Asli Jatim -->
        <label class="flex items-center gap-2 text-xs font-semibold text-gray-700 bg-amber-50 border border-amber-200 px-3 py-2 rounded-xl cursor-pointer">
          <input type="checkbox" v-model="selectedBadgeOnly" class="rounded text-brand-600 focus:ring-brand-500" />
          <span>Hanya Badge Asli Jatim</span>
        </label>
      </div>
    </div>

    <!-- Product Grid -->
    <div>
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-lg font-bold text-gray-900">Katalog Produk Pilihan ({{ filteredProducts.length }})</h2>
      </div>

      <div v-if="filteredProducts.length > 0" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        <ProductCard
          v-for="product in filteredProducts"
          :key="product.id"
          :product="product"
        />
      </div>

      <div v-else class="text-center py-16 bg-white rounded-2xl border border-gray-100">
        <p class="text-gray-500 text-sm">Tidak ada produk yang cocok dengan filter pencarian Anda.</p>
      </div>
    </div>
  </div>
</template>
