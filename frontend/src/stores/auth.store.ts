import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, AuthResponse } from '@/types'
import { apiClient } from '@/api/client'

export const useAuthStore = defineStore('auth', () => {
  const getInitialUser = (): User | null => {
    try {
      const saved = localStorage.getItem('umkm_user')
      return saved ? JSON.parse(saved) : null
    } catch {
      return null
    }
  }

  const user = ref<User | null>(getInitialUser())
  const accessToken = ref<string | null>(localStorage.getItem('umkm_access_token'))

  const isAuthenticated = computed(() => !!accessToken.value && !!user.value)
  const isAdmin = computed(() => user.value?.role === 'Admin')
  const isSeller = computed(() => user.value?.role === 'Seller')
  const isCustomer = computed(() => user.value?.role === 'Customer')

  async function login(credentials: { email: string; password: string }) {
    const res = await apiClient.post<AuthResponse>('/auth/login', credentials)
    accessToken.value = res.data.accessToken
    user.value = res.data.user

    localStorage.setItem('umkm_access_token', res.data.accessToken)
    localStorage.setItem('umkm_user', JSON.stringify(res.data.user))
    return res.data
  }

  async function register(payload: {
    fullName: string
    email: string
    password: string
    confirmPassword: string
    phoneNumber: string
  }) {
    const res = await apiClient.post('/auth/register', payload)
    return res.data
  }

  async function logout() {
    try {
      await apiClient.post('/auth/logout')
    } catch {
      // ignore
    } finally {
      accessToken.value = null
      user.value = null
      localStorage.removeItem('umkm_access_token')
      localStorage.removeItem('umkm_user')
    }
  }

  return {
    user,
    accessToken,
    isAuthenticated,
    isAdmin,
    isSeller,
    isCustomer,
    login,
    register,
    logout
  }
})
