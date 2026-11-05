# ADR 004: Penanganan Webhook Pembayaran Midtrans (Verifikasi Signature & Idempotensi)

## Konteks
Penyedia gerbang pembayaran (Payment Gateway) seperti Midtrans mengirimkan notifikasi status pembayaran (HTTP POST callback/webhook) secara asynchronous. Jaringan internet publik memiliki risiko pengiriman duplikat (at-least-once delivery retry), pengiriman di luar urutan (out-of-order execution), serta potensi pemalsuan paket webhook oleh pihak jahat (man-in-the-middle / spoofing).

## Keputusan
Kami mengimplementasikan tiga pilar keamanan dan reliabilitas pada webhook Midtrans:

1. **Verifikasi Signature Kriptografis (SHA-512)**:
   - Midtrans menyertakan field `signature_key` pada payload notifikasi.
   - Server menghitung ulang hash SHA-512 dari kombinasi:
     `SHA512(order_id + status_code + gross_amount + ServerKey)`
   - Jika hash yang dihitung tidak identik dengan `signature_key` dari Midtrans, request langsung ditolak dengan status HTTP 400/401 tanpa mengeksekusi logika apapun.

2. **Jaminan Idempotensi via Tabel PaymentLog**:
   - Kolom `TransactionId` pada tabel `PaymentLogs` diberi indeks unik (`IX_PaymentLogs_TransactionId UNIQUE`).
   - Sebelum memperbarui status pesanan, sistem memeriksa apakah `TransactionId` dan `Status` tersebut telah berhasil diproses sebelumnya.
   - Jika notifikasi duplikat terdeteksi, sistem segera mengembalikan status HTTP 200 OK secara idempotent tanpa melakukan mutasi status atau stok ulang.

3. **Status Mapping & Rollback Otomatis**:
   - Status `settlement` atau `capture` (dengan fraud_status `accept`): Pesanan diubah menjadi `Paid`, mencatat waktu `PaidAt`.
   - Status `expire`, `cancel`, atau `deny`: Pesanan diubah menjadi `Expired` atau `Cancelled`, dan background job atau webhook memicu rollback (pengembalian) stok ke produk terkait.

## Konsekuensi & Trade-off
- **Kelebihan**:
  - Kebal terhadap serangan pemalsuan status pembayaran.
  - Aman dari duplikasi saldo atau ganda status pesanan akibat jaringan tidak stabil.
  - Riwayat notifikasi mentah tersimpan lengkap di tabel `PaymentLogs` untuk kebutuhan audit trail keuangan.
- **Kekurangan**:
  - Diperlukan satu query ekstra ke tabel `PaymentLogs` pada setiap incoming webhook (namun lookup berbasis index unik sangat cepat, < 1ms).
