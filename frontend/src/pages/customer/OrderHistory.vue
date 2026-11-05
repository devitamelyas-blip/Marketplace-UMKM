<script setup lang="ts">
import { ref } from 'vue'
import { formatCurrency, formatDate, getOrderStatusBadge } from '@/utils/format'
import type { OrderStatus } from '@/types'

const orders = ref([
  {
    id: 1,
    orderNumber: 'ORD-20261105-001',
    status: 'Paid' as OrderStatus,
    totalAmount: 115000,
    shippingCourier: 'JNE',
    shippingService: 'REG',
    createdAt: new Date().toISOString(),
    productSummary: 'Bumbu Rawon Khas Surabaya (2 unit)'
  },
  {
    id: 2,
    orderNumber: 'ORD-20261104-002',
    status: 'Shipped' as OrderStatus,
    totalAmount: 435000,
    shippingCourier: 'SiCepat',
    shippingService: 'REG',
    createdAt: new Date(Date.now() - 86400000).toISOString(),
    productSummary: 'Kain Batik Tulis Sidoarjo Motif Beras Wutah (1 unit)'
  }
])
</script>

<template>
  <div class="max-w-4xl mx-auto space-y-6">
    <h1 class="text-2xl font-black text-gray-900">Riwayat Pesanan</h1>

    <div class="space-y-4">
      <div
        v-for="order in orders"
        :key="order.id"
        class="bg-white p-6 rounded-2xl border border-gray-100 shadow-sm space-y-4"
      >
        <div class="flex flex-wrap items-center justify-between gap-2 border-b border-gray-100 pb-3">
          <div>
            <span class="font-extrabold text-sm text-gray-900">{{ order.orderNumber }}</span>
            <span class="text-xs text-gray-400 block">{{ formatDate(order.createdAt) }}</span>
          </div>

          <span
            class="px-2.5 py-1 rounded-full text-xs font-semibold"
            :class="getOrderStatusBadge(order.status).bg + ' ' + getOrderStatusBadge(order.status).text"
          >
            {{ getOrderStatusBadge(order.status).label }}
          </span>
        </div>

        <div class="text-sm font-medium text-gray-800">
          {{ order.productSummary }}
        </div>

        <div class="flex items-center justify-between text-xs pt-2">
          <span class="text-gray-500">Kurir: {{ order.shippingCourier }} - {{ order.shippingService }}</span>
          <span class="font-extrabold text-sm text-brand-600">{{ formatCurrency(order.totalAmount) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
