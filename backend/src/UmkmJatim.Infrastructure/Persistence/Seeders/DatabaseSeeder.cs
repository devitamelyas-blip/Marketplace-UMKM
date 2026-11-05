using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UmkmJatim.Domain.Entities;

namespace UmkmJatim.Infrastructure.Persistence.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            await context.Database.EnsureCreatedAsync();

            // 1. Roles
            string[] roles = ["Admin", "Seller", "Customer"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            // 2. Admin User
            var adminEmail = "admin@umkmjatim.id";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrator UMKM Jatim",
                    Role = "Admin",
                    PhoneNumber = "081234567890",
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await userManager.CreateAsync(adminUser, "Admin@12345");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // 3. Categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new() { Name = "Makanan & Minuman Khas", Slug = "makanan-minuman", Description = "Kuliner otentik khas Jawa Timur seperti rawon, petis, dan olahan tradisional." },
                    new() { Name = "Kerajinan Tangan", Slug = "kerajinan-tangan", Description = "Karya seni kriya bambu, kayu, kulit, dan keramik khas seniman Jawa Timur." },
                    new() { Name = "Batik & Tenun", Slug = "batik-tenun", Description = "Batik tulis dan cap khas Madura, Sidoarjo, Mojokerto, dan Banyuwangi." },
                    new() { Name = "Kopi & Minuman Herbal", Slug = "kopi-herbal", Description = "Biji kopi lereng Ijen, Argopuro, dan olahan herbal tradisional." },
                    new() { Name = "Bumbu & Rempah", Slug = "bumbu-rempah", Description = "Rempah asli pilihan untuk racikan masakan Nusantara otentik." },
                    new() { Name = "Oleh-Oleh Tradisional", Slug = "oleh-oleh", Description = "Camilan renyah, kerupuk, dan jajanan favorit khas berbagai kabupaten di Jatim." }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            var catList = await context.Categories.ToListAsync();
            var catMakanan = catList.First(c => c.Slug == "makanan-minuman");
            var catKerajinan = catList.First(c => c.Slug == "kerajinan-tangan");
            var catBatik = catList.First(c => c.Slug == "batik-tenun");
            var catKopi = catList.First(c => c.Slug == "kopi-herbal");
            var catBumbu = catList.First(c => c.Slug == "bumbu-rempah");
            var catOlehOleh = catList.First(c => c.Slug == "oleh-oleh");

            // 4. Seller Users & Shops
            var sellerConfigs = new[]
            {
                (Email: "toko.surabaya@umkmjatim.id", Name: "Pak Bambang Santoso", ShopName: "Pawon Arek Suroboyo", City: "Surabaya", Desc: "Spesialis bumbu rawon, petis hitam kualitas super, dan olahan khas pesisir Surabaya."),
                (Email: "toko.malang@umkmjatim.id", Name: "Ibu Sri Wahyuni", ShopName: "Kendedes Craft & Agro", City: "Malang", Desc: "Pusat oleh-oleh apel segar, keripik tempe Sanan, dan kerajinan keramik Dinoyo."),
                (Email: "toko.sidoarjo@umkmjatim.id", Name: "H. Mochamad Ali", ShopName: "Sentra Batik & Bandeng Sidoarjo", City: "Sidoarjo", Desc: "Batik motif udang bandeng asli Jetis dan olahan bandeng cabut duri renyah."),
                (Email: "toko.mojokerto@umkmjatim.id", Name: "Mas Raden Wijaya", ShopName: "Majapahit Heritage Souvenir", City: "Mojokerto", Desc: "Kerajinan pahat batu Trowulan Majapahit, onde-onde legendaris, dan batik Surya Majapahit."),
                (Email: "toko.jember@umkmjatim.id", Name: "Cak Hendra Ijen", ShopName: "Kopi Lereng Raung & Ijen", City: "Jember", Desc: "Kopi specialty Arabika Ijen Raung bersertifikat Indikasi Geografis dan tape Bondowoso manis.")
            };

            var shops = new List<Shop>();
            foreach (var cfg in sellerConfigs)
            {
                var sellerUser = await userManager.FindByEmailAsync(cfg.Email);
                if (sellerUser == null)
                {
                    sellerUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = cfg.Email,
                        Email = cfg.Email,
                        FullName = cfg.Name,
                        Role = "Seller",
                        PhoneNumber = "081398765432",
                        IsActive = true,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await userManager.CreateAsync(sellerUser, "Seller@12345");
                    await userManager.AddToRoleAsync(sellerUser, "Seller");

                    var shop = new Shop
                    {
                        OwnerId = sellerUser.Id,
                        Name = cfg.ShopName,
                        Slug = cfg.ShopName.ToLower().Replace(" ", "-").Replace("&", "dan"),
                        Description = cfg.Desc,
                        City = cfg.City,
                        Province = "Jawa Timur",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    context.Shops.Add(shop);
                    await context.SaveChangesAsync();
                    shops.Add(shop);
                }
                else
                {
                    var existingShop = await context.Shops.FirstOrDefaultAsync(s => s.OwnerId == sellerUser.Id);
                    if (existingShop != null) shops.Add(existingShop);
                }
            }

            // 5. Customer Users
            var customerEmails = new[] { "customer1@test.id", "customer2@test.id", "customer3@test.id" };
            foreach (var cEmail in customerEmails)
            {
                var custUser = await userManager.FindByEmailAsync(cEmail);
                if (custUser == null)
                {
                    custUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = cEmail,
                        Email = cEmail,
                        FullName = $"Pelanggan Setia ({cEmail.Split('@')[0]})",
                        Role = "Customer",
                        PhoneNumber = "085712345678",
                        IsActive = true,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await userManager.CreateAsync(custUser, "Customer@12345");
                    await userManager.AddToRoleAsync(custUser, "Customer");

                    // Beri alamat contoh
                    context.Addresses.Add(new Address
                    {
                        UserId = custUser.Id,
                        Label = "Rumah",
                        RecipientName = custUser.FullName,
                        PhoneNumber = custUser.PhoneNumber,
                        Street = "Jl. Darmo No. 45, Wonokromo",
                        City = "Surabaya",
                        Province = "Jawa Timur",
                        PostalCode = "60241",
                        IsDefault = true
                    });
                    await context.SaveChangesAsync();
                }
            }

            // 6. 30 Real Jawa Timur Products
            if (!await context.Products.AnyAsync() && shops.Count >= 5)
            {
                var sbyShop = shops[0];
                var mlgShop = shops[1];
                var sdaShop = shops[2];
                var mjkShop = shops[3];
                var jbrShop = shops[4];

                var products = new List<Product>
                {
                    // Surabaya (6 produk)
                    new() { ShopId = sbyShop.Id, CategoryId = catBumbu.Id, Name = "Bumbu Rawon Khas Surabaya Ny. Giok", Slug = "bumbu-rawon-khas-surabaya", Description = "Bumbu rawon kluwek hitam pekat otentik resep turun-temurun tanpa bahan pengawet. Kemasan botol kaca kedap udara 250gr.", Price = 38000m, Stock = 85, OriginCity = "Surabaya", WeightGrams = 300, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = sbyShop.Id, CategoryId = catMakanan.Id, Name = "Petis Udang Super Asli Kenjeran", Slug = "petis-udang-super-kenjeran", Description = "Petis udang grade A warna hitam mengkilap, rasa gurih manis alami tanpa campuran tepung berlebih. Sangat cocok untuk tahu tek dan rujak.", Price = 45000m, Stock = 60, OriginCity = "Surabaya", WeightGrams = 250, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = sbyShop.Id, CategoryId = catMakanan.Id, Name = "Bumbu Rujak Cingur Komplit Surabaya", Slug = "bumbu-rujak-cingur-komplit", Description = "Racikan petis Madura dan Surabaya dipadu kacang tanah sangrai dan pisang klutuk. Tinggal seduh air hangat.", Price = 32000m, Stock = 50, OriginCity = "Surabaya", WeightGrams = 200, IsJatimBadge = false },
                    new() { ShopId = sbyShop.Id, CategoryId = catOlehOleh.Id, Name = "Lapis Kukus Pahlawan Surabaya Original", Slug = "lapis-kukus-pahlawan-original", Description = "Kue lapis kukus lembut berbahan singkong pilihan dengan taburan keju melimpah di atasnya. Tahan 4 hari di suhu ruang.", Price = 37000m, Stock = 40, OriginCity = "Surabaya", WeightGrams = 500, IsJatimBadge = false },
                    new() { ShopId = sbyShop.Id, CategoryId = catOlehOleh.Id, Name = "Almond Crispy Cheese Surabaya Premium", Slug = "almond-crispy-cheese-surabaya", Description = "Biskuit renyah tipis bertabur irisan almond dan keju gurih. Cemilan paling dicari pelancong di Surabaya.", Price = 55000m, Stock = 75, OriginCity = "Surabaya", WeightGrams = 350, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = sbyShop.Id, CategoryId = catMakanan.Id, Name = "Sambal Klotok Arek Surabaya Pedas Nampol", Slug = "sambal-klotok-surabaya", Description = "Sambal uleg tradisional dengan suwiran ikan klotok asin gurih dan cabai rawit pedas segar khas Jawa Timur.", Price = 28000m, Stock = 90, OriginCity = "Surabaya", WeightGrams = 200, IsJatimBadge = false },

                    // Malang (6 produk)
                    new() { ShopId = mlgShop.Id, CategoryId = catOlehOleh.Id, Name = "Keripik Apel Manalagi Malang Super Crispy", Slug = "keripik-apel-manalagi-malang", Description = "Keripik apel diproses vacuum frying suhu rendah sehingga aroma segar dan rasa manis-asam apel Malang tetap terjaga.", Price = 25000m, Stock = 120, OriginCity = "Malang", WeightGrams = 150, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = mlgShop.Id, CategoryId = catOlehOleh.Id, Name = "Keripik Tempe Sanan Aneka Rasa Original", Slug = "keripik-tempe-sanan-malang", Description = "Keripik tempe legendaris sentra Sanan Malang dengan potongan tipis garing dan bumbu daun jeruk wangi.", Price = 18000m, Stock = 150, OriginCity = "Malang", WeightGrams = 200, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = mlgShop.Id, CategoryId = catKerajinan.Id, Name = "Topeng Malangan Kayu Ukir Patih Karakter Gagah", Slug = "topeng-malangan-kayu-ukir", Description = "Topeng kayu mahoni pahatan tangan seniman Kedungmonggo Malang, dilapisi cat enamel tradisional tahan cuaca.", Price = 275000m, Stock = 15, OriginCity = "Malang", WeightGrams = 600, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = mlgShop.Id, CategoryId = catKerajinan.Id, Name = "Cangkir Keramik Dinoyo Malang Handmade", Slug = "cangkir-keramik-dinoyo-handmade", Description = "Cangkir keramik stoneware pembakaran 1250C dengan glasir motif daun apel khas sentra Dinoyo Malang.", Price = 65000m, Stock = 35, OriginCity = "Malang", WeightGrams = 400, IsJatimBadge = false },
                    new() { ShopId = mlgShop.Id, CategoryId = catMakanan.Id, Name = "Sari Apel Malang Murni Tanpa Pemanis Buatan (1 Liter)", Slug = "sari-apel-malang-murni", Description = "Minuman sari buah apel segar hasil panen petani Batu Malang, kaya antioksidan dan menyegarkan saat dingin.", Price = 30000m, Stock = 65, OriginCity = "Malang", WeightGrams = 1100, IsJatimBadge = false },
                    new() { ShopId = mlgShop.Id, CategoryId = catOlehOleh.Id, Name = "Bakpia Telo Ungu Malang Gurih Legit", Slug = "bakpia-telo-ungu-malang", Description = "Kue pia berkulit renyah dengan isian pasta ubi ungu lokal Malang yang lembut dan legit.", Price = 35000m, Stock = 50, OriginCity = "Malang", WeightGrams = 450, IsJatimBadge = false },

                    // Sidoarjo (6 produk)
                    new() { ShopId = sdaShop.Id, CategoryId = catBatik.Id, Name = "Kain Batik Tulis Sidoarjo Motif Beras Wutah", Slug = "batik-tulis-sidoarjo-beras-wutah", Description = "Kain katun primissima 2.2 meter batik tulis halus motif Beras Wutah pesisiran khas kampung Jetis Sidoarjo.", Price = 420000m, Stock = 12, OriginCity = "Sidoarjo", WeightGrams = 350, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = sdaShop.Id, CategoryId = catBatik.Id, Name = "Kemeja Batik Cap Pria Motif Bandeng Udang", Slug = "kemeja-batik-cap-bandeng-udang", Description = "Kemeja lengan pendek bahan katun adem dengan corak ikonik kabupaten Sidoarjo yaitu udang dan bandeng.", Price = 175000m, Stock = 25, OriginCity = "Sidoarjo", WeightGrams = 280, IsJatimBadge = false },
                    new() { ShopId = sdaShop.Id, CategoryId = catMakanan.Id, Name = "Bandeng Asap Sidoarjo Spesial Sambal Petis", Slug = "bandeng-asap-sidoarjo-spesial", Description = "Bandeng tambak segar diasap menggunakan batok kelapa kering pilihan, menghasilkan aroma wangi dan tekstur daging lembut.", Price = 75000m, Stock = 30, OriginCity = "Sidoarjo", WeightGrams = 700, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = sdaShop.Id, CategoryId = catOlehOleh.Id, Name = "Kerupuk Udang Super Mentah Ny. Siok Sidoarjo", Slug = "kerupuk-udang-super-sidoarjo", Description = "Kerupuk udang kualitas ekspor dengan kandungan daging udang lebih dari 60%, mekar renyah sempurna saat digoreng.", Price = 68000m, Stock = 90, OriginCity = "Sidoarjo", WeightGrams = 500, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = sdaShop.Id, CategoryId = catKerajinan.Id, Name = "Tas Kulit Asli Tanggulangin Sidoarjo Model Tote", Slug = "tas-kulit-asli-tanggulangin-tote", Description = "Tas jinjing wanita bahan kulit sapi pull-up asli hasil kerajinan pengrajin Tanggulangin Sidoarjo dengan jahitan kokoh.", Price = 385000m, Stock = 18, OriginCity = "Sidoarjo", WeightGrams = 800, IsJatimBadge = false },
                    new() { ShopId = sdaShop.Id, CategoryId = catMakanan.Id, Name = "Otak-Otak Bandeng Sidoarjo Non-Duri Siap Santap", Slug = "otak-otak-bandeng-sidoarjo", Description = "Daging bandeng dicincang halus dan dibumbui santan gurih, dimasukkan kembali ke kulit bandeng lalu dikukus matang.", Price = 42000m, Stock = 45, OriginCity = "Sidoarjo", WeightGrams = 400, IsJatimBadge = false },

                    // Mojokerto (6 produk)
                    new() { ShopId = mjkShop.Id, CategoryId = catKerajinan.Id, Name = "Miniatur Candi Tikus Majapahit Batu Andesit", Slug = "miniatur-candi-tikus-majapahit", Description = "Pahatan miniatur situs purbakala Majapahit Trowulan dari batu andesit alami, cocok untuk pajangan meja kerja.", Price = 180000m, Stock = 20, OriginCity = "Mojokerto", WeightGrams = 1200, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = mjkShop.Id, CategoryId = catBatik.Id, Name = "Batik Tulis Surya Majapahit Warna Alam", Slug = "batik-tulis-surya-majapahit", Description = "Batik tulis klasik dengan lambang kerajaan Majapahit diwarnai menggunakan ekstrak kulit kayu tingi dan jolawe.", Price = 550000m, Stock = 8, OriginCity = "Mojokerto", WeightGrams = 320, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = mjkShop.Id, CategoryId = catOlehOleh.Id, Name = "Onde-Onde Legendaris Mojokerto Isi Kacang Hijau (Isi 10)", Slug = "onde-onde-legendaris-mojokerto", Description = "Onde-onde renyah gurih bertabur wijen penuh dengan pasta kacang hijau manis pulen. Dikemas vakum khusus perjalanan.", Price = 40000m, Stock = 35, OriginCity = "Mojokerto", WeightGrams = 600, IsJatimBadge = false },
                    new() { ShopId = mjkShop.Id, CategoryId = catKerajinan.Id, Name = "Anyaman Bambu Trowulan Tempat Wadah Serbaguna", Slug = "anyaman-bambu-trowulan-wadah", Description = "Keranjang anyaman bambu apus halus dengan finishing vernis ramah lingkungan buatan perajin desa Trowulan.", Price = 48000m, Stock = 50, OriginCity = "Mojokerto", WeightGrams = 300, IsJatimBadge = false },
                    new() { ShopId = mjkShop.Id, CategoryId = catMakanan.Id, Name = "Kecap Manis Tradisional Cap Jeruk Pecel Mojokerto", Slug = "kecap-manis-cap-jeruk-pecel", Description = "Kecap manis kental dari kedelai hitam lokal dan gula kelapa murni resep kuno sejak tahun 1937 di Mojokerto.", Price = 26000m, Stock = 70, OriginCity = "Mojokerto", WeightGrams = 750, IsJatimBadge = false },
                    new() { ShopId = mjkShop.Id, CategoryId = catOlehOleh.Id, Name = "Keripik Singkong Balado Majapahit Krispi", Slug = "keripik-singkong-balado-majapahit", Description = "Olahan singkong lokal iris tipis digoreng garing renyah dipadukan dengan bumbu cabai rawit pedas manis gurih.", Price = 16000m, Stock = 110, OriginCity = "Mojokerto", WeightGrams = 250, IsJatimBadge = false },

                    // Jember (6 produk)
                    new() { ShopId = jbrShop.Id, CategoryId = catKopi.Id, Name = "Kopi Arabika Java Ijen Raung Single Origin (250g)", Slug = "kopi-arabika-java-ijen-raung", Description = "Biji kopi Arabika petik merah dari lereng vulkanik kawah Ijen. Notes aroma bunga melati, asam sitrun cerah, dan manis karamel.", Price = 78000m, Stock = 80, OriginCity = "Jember", WeightGrams = 280, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = jbrShop.Id, CategoryId = catKopi.Id, Name = "Kopi Robusta Kolong Jember Dark Roast (250g)", Slug = "kopi-robusta-kolong-jember", Description = "Robusta klasik body tebal, aroma cokelat pahit mantap dan kadar kafein seimbang. Cocok untuk kopi tubruk dan es kopi susu.", Price = 42000m, Stock = 95, OriginCity = "Jember", WeightGrams = 280, IsJatimBadge = false },
                    new() { ShopId = jbrShop.Id, CategoryId = catOlehOleh.Id, Name = "Tape Manis Bondowoso Besek Bambu Primadona", Slug = "tape-manis-bondowoso-besek", Description = "Tape singkong kuning mentega fermentasi sempurna dalam kemasan besek bambu harum. Rasa manis berair tanpa serat kasar.", Price = 32000m, Stock = 40, OriginCity = "Jember", WeightGrams = 800, IsJatimBadge = true, BadgeVerifiedAt = DateTime.UtcNow, BadgeVerifiedById = adminUser.Id },
                    new() { ShopId = jbrShop.Id, CategoryId = catOlehOleh.Id, Name = "Suwar-Suwir Jember Rasa Sirsak & Cokelat", Slug = "suwar-suwir-jember-aneka-rasa", Description = "Permen dodol tradisional berbahan dasar tape singkong dengan aneka rasa buah segar khas sentra oleh-oleh Jember.", Price = 22000m, Stock = 100, OriginCity = "Jember", WeightGrams = 300, IsJatimBadge = false },
                    new() { ShopId = jbrShop.Id, CategoryId = catMakanan.Id, Name = "Edamame Jember Beku Kualitas Ekspor Jepang (500g)", Slug = "edamame-jember-beku-ekspor", Description = "Kacang kedelai edamame segar polong isi 3-4 butir dipanen pada tingkat kematangan terbaik di lembah Jember.", Price = 26000m, Stock = 70, OriginCity = "Jember", WeightGrams = 520, IsJatimBadge = false },
                    new() { ShopId = jbrShop.Id, CategoryId = catKopi.Id, Name = "Teh Hitam Kebun Gunung Gambir Jember CTC", Slug = "teh-hitam-gunung-gambir-jember", Description = "Teh hitam celup dari perkebunan peninggalan Belanda di lereng Pegunungan Argopuro Jember dengan aroma wangi pekat.", Price = 24000m, Stock = 85, OriginCity = "Jember", WeightGrams = 150, IsJatimBadge = false }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();

                // Tambahkan primary image placeholder untuk tiap produk
                foreach (var prod in products)
                {
                    context.ProductImages.Add(new ProductImage
                    {
                        ProductId = prod.Id,
                        Url = $"/images/products/{prod.Slug}.jpg",
                        SortOrder = 0,
                        IsPrimary = true
                    });
                }
                await context.SaveChangesAsync();

                logger.LogInformation("Database seeder berhasil mengisi 30 produk khas Jawa Timur dan 5 toko UMKM!");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gagal menjalankan database seeder.");
            throw;
        }
    }
}
