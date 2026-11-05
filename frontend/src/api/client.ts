import axios from 'axios'

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: {
    'Content-Type': 'application/json'
  },
  withCredentials: true
})

// Request interceptor to attach JWT token
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('umkm_access_token')
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Response interceptor for token refresh
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config
    if (error.response?.status === 401 && !originalRequest._retry && !originalRequest.url?.includes('/auth/login')) {
      originalRequest._retry = true
      try {
        const refreshResponse = await axios.post('/api/auth/refresh', {}, { withCredentials: true })
        const newToken = refreshResponse.data.accessToken
        localStorage.setItem('umkm_access_token', newToken)
        originalRequest.headers.Authorization = `Bearer ${newToken}`
        return apiClient(originalRequest)
      } catch (refreshErr) {
        localStorage.removeItem('umkm_access_token')
        localStorage.removeItem('umkm_user')
        window.location.href = '/login'
        return Promise.reject(refreshErr)
      }
    }
    return Promise.reject(error)
  }
)
