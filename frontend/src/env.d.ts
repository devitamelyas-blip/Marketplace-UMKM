/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string
  readonly VITE_MIDTRANS_CLIENT_KEY: string
  readonly VITE_MIDTRANS_SNAP_URL: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}

declare interface Window {
  snap?: {
    pay: (token: string, options: {
      onSuccess?: (result: unknown) => void
      onPending?: (result: unknown) => void
      onError?: (result: unknown) => void
      onClose?: () => void
    }) => void
  }
}
