<script setup lang="ts">
import { useUiStore } from '@/stores/ui.store'

const uiStore = useUiStore()
</script>

<template>
  <div class="fixed bottom-5 right-5 z-50 flex flex-col gap-2.5 max-w-sm w-full pointer-events-none">
    <div
      v-for="toast in uiStore.toasts"
      :key="toast.id"
      class="pointer-events-auto p-4 rounded-xl shadow-lg border text-sm font-medium transition-all duration-300 flex items-center justify-between"
      :class="{
        'bg-emerald-50 text-emerald-800 border-emerald-200': toast.type === 'success',
        'bg-rose-50 text-rose-800 border-rose-200': toast.type === 'error',
        'bg-blue-50 text-blue-800 border-blue-200': toast.type === 'info',
        'bg-amber-50 text-amber-800 border-amber-200': toast.type === 'warning',
      }"
    >
      <span>{{ toast.message }}</span>
      <button @click="uiStore.removeToast(toast.id)" class="ml-3 text-gray-400 hover:text-gray-700 font-bold">&times;</button>
    </div>
  </div>
</template>
