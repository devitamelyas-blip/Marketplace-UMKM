# ADR 002: Autentikasi JWT dengan Access Token & HttpOnly Cookie Refresh Token

## Konteks
Aplikasi berbasis Single Page Application (Vue 3) membutuhkan autentikasi yang aman dan tidak membebani server dengan state sesi di memory (stateless). Di sisi lain, menyimpan token autentikasi di `localStorage` pada browser sangat rentan terhadap serangan Cross-Site Scripting (XSS).

## Keputusan
Kami mengimplementasikan strategi token ganda:
1. **Access Token (JWT)**:
   - Masa berlaku singkat (15 menit).
   - Membawa klaim pengguna (UserId, Email, FullName, Role, Jti).
   - Disimpan di memori runtime aplikasi (Pinia store) dan dikirim via header HTTP `Authorization: Bearer <token>`.
2. **Refresh Token**:
   - Masa berlaku panjang (7 hari).
   - Berupa string kriptografi acak aman (64 bytes CSPRNG), disimpan di database dengan status pelacakan (`IsRevoked`, `ExpiresAt`, `ReplacedByToken`).
   - Dikirimkan ke browser menggunakan atribut cookie **HttpOnly**, **SameSite=Lax/Strict**, dan **Secure** (pada mode HTTPS).
   - Dengan HttpOnly, skrip JavaScript berbahaya (XSS) sama sekali tidak dapat membaca atau mencuri refresh token.
3. **Mekanisme Silent Refresh**:
   - Axios interceptor pada frontend secara transparan menangkap respon status 401 Unauthorized, memicu endpoint `/api/auth/refresh`, memperbarui access token di memori, dan mengulang request asli tanpa mengganggu alur pengguna.

## Konsekuensi & Trade-off
- **Kelebihan**:
  - Pertahanan berlapis terhadap XSS (pencurian sesi permanen dicegah).
  - Skalabilitas horizontal server terjaga karena access token diverifikasi secara kriptografis tanpa query database berulang.
  - Sesi dapat dicabut secara instan di sisi server (revocation) dengan menandai token di tabel `RefreshTokens`.
- **Kekurangan**:
  - Perlu penanganan konfigurasi CORS yang ketat (`AllowCredentials()` dan origin terdaftar eksplisit).
  - Butuh alur rotasi refresh token untuk mendeteksi token reuse attack.

