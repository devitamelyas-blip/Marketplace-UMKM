import type { Config } from 'tailwindcss'

export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        brand: {
          50: '#fdf4f3',
          100: '#fbe8e6',
          500: '#c2410c', // Terracotta Jawa Timuran
          600: '#9a3412',
          700: '#7c2d12',
        },
        jatim: {
          gold: '#d97706',
          green: '#15803d'
        }
      }
    },
  },
  plugins: [],
} satisfies Config
