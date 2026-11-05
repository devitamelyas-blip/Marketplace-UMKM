import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/pages/catalog/Home.vue'
import Cart from '@/pages/cart/Cart.vue'
import Checkout from '@/pages/checkout/Checkout.vue'
import OrderHistory from '@/pages/customer/OrderHistory.vue'
import Login from '@/pages/auth/Login.vue'
import Register from '@/pages/auth/Register.vue'
import SellerDashboard from '@/pages/seller/SellerDashboard.vue'
import AdminDashboard from '@/pages/admin/AdminDashboard.vue'

const routes = [
  { path: '/', component: Home },
  { path: '/keranjang', component: Cart },
  { path: '/checkout', component: Checkout },
  { path: '/pesanan', component: OrderHistory },
  { path: '/login', component: Login },
  { path: '/daftar', component: Register },
  { path: '/seller/dashboard', component: SellerDashboard },
  { path: '/admin/dashboard', component: AdminDashboard }
]

export const router = createRouter({
  history: createWebHistory(),
  routes
})
