import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface Toast {
  id: string
  type: 'success' | 'error' | 'info' | 'warning'
  message: string
}

export const useUiStore = defineStore('ui', () => {
  const toasts = ref<Toast[]>([])

  function showToast(type: Toast['type'], message: string, duration = 3500) {
    const id = Math.random().toString(36).substring(2, 9)
    toasts.value.push({ id, type, message })

    setTimeout(() => {
      removeToast(id)
    }, duration)
  }

  function removeToast(id: string) {
    toasts.value = toasts.value.filter(t => t.id !== id)
  }

  return {
    toasts,
    showToast,
    removeToast
  }
})
