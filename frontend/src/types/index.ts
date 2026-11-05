export type Role = 'Admin' | 'Seller' | 'Customer'

export interface User {
  id: string
  email: string
  fullName: string
  role: Role
  phoneNumber?: string
  avatarUrl?: string
  isActive: boolean
}

export interface AuthResponse {
  accessToken: string
  refreshToken: string
  expiresAt: string
  user: User
}

export interface Address {
  id: number
  label: string
  recipientName: string
  phoneNumber: string
  street: string
  city: string
  province: string
  postalCode: string
  isDefault: boolean
}

export interface Category {
  id: number
  name: string
  slug: string
  description?: string
}

export interface Product {
  id: number
  shopId: number
  categoryId: number
  name: string
  slug: string
  description: string
  price: number
  stock: number
  originCity: string
  originProvince: string
  weightGrams: number
  isActive: boolean
  isJatimBadge: boolean
  categoryName?: string
  shopName?: string
  primaryImageUrl?: string
}

export interface CartItem {
  id: number
  productId: number
  name: string
  price: number
  quantity: number
  stock: number
  imageUrl?: string
  shopName: string
  originCity: string
}

export type OrderStatus = 'Pending' | 'Paid' | 'Processed' | 'Shipped' | 'Completed' | 'Cancelled' | 'Expired'

export interface Order {
  id: number
  orderNumber: string
  status: OrderStatus
  subTotal: number
  shippingCost: number
  totalAmount: number
  shippingCourier: string
  shippingService: string
  paymentDeadline?: string
  paidAt?: string
  createdAt: string
  items: OrderItem[]
  shippingAddress: Address
}

export interface OrderItem {
  id: number
  productId: number
  productName: string
  productImageUrl?: string
  unitPrice: number
  quantity: number
  subTotal: number
  shopName: string
}

export interface ShippingOption {
  courier: string
  service: string
  description: string
  cost: number
  estimateDays: number
}
