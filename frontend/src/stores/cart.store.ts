import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { CartItem, Product } from '@/types'

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([])

  const totalCount = computed(() => items.value.reduce((acc, item) => acc + item.quantity, 0))
  const totalPrice = computed(() => items.value.reduce((acc, item) => acc + (item.price * item.quantity), 0))

  function addToCart(product: Product, quantity = 1) {
    const existing = items.value.find(i => i.productId === product.id)
    if (existing) {
      existing.quantity = Math.min(existing.stock, existing.quantity + quantity)
    } else {
      items.value.push({
        id: Date.now(),
        productId: product.id,
        name: product.name,
        price: product.price,
        quantity,
        stock: product.stock,
        imageUrl: product.primaryImageUrl,
        shopName: product.shopName || 'Toko UMKM Jatim',
        originCity: product.originCity
      })
    }
  }

  function updateQuantity(productId: number, quantity: number) {
    const item = items.value.find(i => i.productId === productId)
    if (item) {
      if (quantity <= 0) {
        removeFromCart(productId)
      } else {
        item.quantity = Math.min(item.stock, quantity)
      }
    }
  }

  function removeFromCart(productId: number) {
    items.value = items.value.filter(i => i.productId !== productId)
  }

  function clearCart() {
    items.value = []
  }

  return {
    items,
    totalCount,
    totalPrice,
    addToCart,
    updateQuantity,
    removeFromCart,
    clearCart
  }
})
