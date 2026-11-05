# ADR 003: Kontrol Konkurensi Stok Menggunakan Optimistic Concurrency (RowVersion)

## Konteks
Pada skenario marketplace saat flash sale atau penjualan produk UMKM khas yang stoknya terbatas (misal: hanya tersedia 15 unit kain batik tulis langka), rentan terjadi *race condition* jika beberapa pembeli melakukan checkout secara bersamaan (*overselling* atau stok minus).

Dua pendekatan umum dalam basis data relasional:
1. **Pessimistic Concurrency**: Mengunci baris database secara eksklusif menggunakan transaksi `SELECT ... WITH (UPDLOCK, ROWLOCK)`.
2. **Optimistic Concurrency**: Mengizinkan pembacaan simultan dan mendeteksi perubahan data saat penulisan (`RowVersion` / timestamp token).

## Keputusan
Kami memilih **Optimistic Concurrency Control** menggunakan kolom `RowVersion` (`byte[]` dengan anotasi `[Timestamp]` di EF Core / SQL Server):
1. Setiap entitas `Product` memiliki kolom `RowVersion` yang secara otomatis diperbarui oleh SQL Server saat baris mengalami mutasi.
2. Saat proses pemotongan stok pada transaksi checkout, EF Core mengeksekusi:
   ```sql
   UPDATE Products SET Stock = Stock - @Qty, ... 
   WHERE Id = @Id AND RowVersion = @OriginalRowVersion;
   ```
3. Jika baris telah diubah oleh transaksi lain dalam waktu yang sama, jumlah baris yang terpengaruh bernilai 0, sehingga EF Core melempar `DbUpdateConcurrencyException`.
4. Lapisan Application/API menangkap exception ini dan memetakan menjadi HTTP 409 Conflict (ProblemDetails) atau memicu mekanisme retry otomatis dengan exponential backoff jika stok masih mencukupi.

## Perbandingan Trade-off

| Parameter | Optimistic Concurrency (Dipilih) | Pessimistic Locking (UPDLOCK) |
|---|---|---|
| **Database Row Locking** | Tidak ada row lock selama browsing & kalkulasi checkout. | Mengunci baris tabel selama transaksi berlangsung. |
| **Throughput & Skalabilitas** | Sangat tinggi, ideal untuk beban baca yang tinggi (katalog e-commerce). | Menurun drastis saat banyak transaksi membaca baris yang sama. |
| **Resiko Deadlock** | Minim sekali. | Rentan terjadi deadlock jika urutan penguncian item berbeda. |
| **Penanganan Konflik** | Membutuhkan penanganan di application code (retry atau pesan ke user). | Otomatis diantrekan oleh lock engine database. |

## Kesimpulan
Pendekatan Optimistic Concurrency memberikan throughput baca dan tulis yang optimal untuk arsitektur e-commerce modern, menjaga integritas data tanpa mengorbankan performa database secara keseluruhan.
