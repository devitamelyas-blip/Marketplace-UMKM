import type { OrderStatus } from '@/types'

export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('id-ID', {
    style: 'currency',
    currency: 'IDR',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

export function formatDate(dateStr: string): string {
  if (!dateStr) return '-'
  return new Intl.DateTimeFormat('id-ID', {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(new Date(dateStr))
}

export function getOrderStatusBadge(status: OrderStatus): { label: string; bg: string; text: string } {
  switch (status) {
    case 'Pending':
      return { label: 'Menunggu Pembayaran', bg: 'bg-amber-100', text: 'text-amber-800' }
    case 'Paid':
      return { label: 'Dibayar', bg: 'bg-blue-100', text: 'text-blue-800' }
    case 'Processed':
      return { label: 'Sedang Diproses', bg: 'bg-purple-100', text: 'text-purple-800' }
    case 'Shipped':
      return { label: 'Dalam Pengiriman', bg: 'bg-indigo-100', text: 'text-indigo-800' }
    case 'Completed':
      return { label: 'Selesai', bg: 'bg-emerald-100', text: 'text-emerald-800' }
    case 'Cancelled':
      return { label: 'Dibatalkan', bg: 'bg-rose-100', text: 'text-rose-800' }
    case 'Expired':
      return { label: 'Kedaluwarsa', bg: 'bg-gray-100', text: 'text-gray-700' }
    default:
      return { label: status, bg: 'bg-gray-100', text: 'text-gray-700' }
  }
}
