<script setup lang="ts">
import AppNavbar from '@/components/layout/AppNavbar.vue'
import AppFooter from '@/components/layout/AppFooter.vue'
import AppToast from '@/components/ui/AppToast.vue'
</script>

<template>
  <div class="min-h-screen flex flex-col bg-gray-50 text-gray-900">
    <AppNavbar />
    <main class="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <router-view />
    </main>
    <AppFooter />
    <AppToast />
  </div>
</template>
