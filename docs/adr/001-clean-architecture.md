# ADR 001: Penerapan Clean Architecture dengan MediatR (CQRS Sederhana)

## Konteks
Pada proyek Marketplace UMKM Jawa Timur, arsitektur harus mendukung pengujian terisolasi (testability), pemisahan tanggung jawab yang tegas (separation of concerns), serta kemudahan perawatan jangka panjang ketika bisnis dan integrasi pihak ketiga (Midtrans, kurir ekspedisi) berkembang.

## Keputusan
Kami memilih mengadopsi **Clean Architecture** yang dipadukan dengan **CQRS sederhana menggunakan MediatR**:
1. **Domain Layer**: Pusat dari seluruh aturan bisnis inti, entitas, exception domain, dan enum. Tidak memiliki dependensi ke layer eksternal maupun framework database.
2. **Application Layer**: Berisi use case bisnis (Commands & Queries), DTO, abstraksi antarmuka (`IApplicationDbContext`, `ITokenService`, `IShippingService`), validator FluentValidation, dan pipeline behaviors (Logging & Validation).
3. **Infrastructure Layer**: Berisi implementasi teknis konkret dari abstraksi Application, termasuk Entity Framework Core 8, ASP.NET Core Identity, layanan token JWT, background service, dan integrasi API pihak ketiga.
4. **API Layer**: Sebagai titik masuk HTTP. Controller dibuat tipis (thin controllers) dan hanya bertindak sebagai mediator yang meneruskan request ke MediatR serta mengembalikan response HTTP standar.

## Konsekuensi & Trade-off
- **Kelebihan**:
  - Logika bisnis dapat diuji secara independen tanpa memerlukan database nyata maupun HTTP context.
  - Perubahan infrastruktur (misalnya migrasi penyimpanan file lokal ke Azure Blob Storage/S3) tidak merusak aturan bisnis di Application maupun Domain.
  - Standarisasi validasi dan penanganan exception terpusat melalui MediatR Pipeline Behavior.
- **Kekurangan**:
  - Terdapat lebih banyak berkas dan proyek (.csproj) dibandingkan struktur monolitik tradisional (misal: folder Models, Views, Controllers standar).
  - Sedikit overhead alokasi memory dari MediatR mediator pipeline (namun terbukti sangat minim pada .NET 8).

