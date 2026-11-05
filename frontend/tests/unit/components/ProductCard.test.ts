import { describe, it, expect, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import ProductCard from '@/components/shared/ProductCard.vue'
import type { Product } from '@/types'

describe('ProductCard.vue', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  const sampleProduct: Product = {
    id: 1,
    shopId: 1,
    categoryId: 1,
    name: 'Bumbu Rawon Khas Surabaya',
    slug: 'bumbu-rawon-khas-surabaya',
    description: 'Bumbu rawon kluwek hitam enak',
    price: 35000,
    stock: 50,
    originCity: 'Surabaya',
    originProvince: 'Jawa Timur',
    weightGrams: 250,
    isActive: true,
    isJatimBadge: true
  }

  it('merender nama produk dan kota asal dengan benar', () => {
    const wrapper = mount(ProductCard, {
      props: { product: sampleProduct }
    })

    expect(wrapper.text()).toContain('Bumbu Rawon Khas Surabaya')
    expect(wrapper.text()).toContain('Surabaya')
  })

  it('menampilkan badge "Asli Jatim" jika isJatimBadge = true', () => {
    const wrapper = mount(ProductCard, {
      props: { product: sampleProduct }
    })

    expect(wrapper.text()).toContain('Asli Jatim')
  })

  it('tidak menampilkan badge "Asli Jatim" jika isJatimBadge = false', () => {
    const wrapper = mount(ProductCard, {
      props: {
        product: { ...sampleProduct, isJatimBadge: false }
      }
    })

    expect(wrapper.text()).not.toContain('Asli Jatim')
  })
})
