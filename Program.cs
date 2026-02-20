/*using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Hizmeti sisteme kaydediyoruz
builder.Services.AddSingleton<HavaDurumuMotoru>();
// Veritabanı dosyasının adını belirliyoruz
var connectionString = "Data Source=hava.db";
// Veritabanı servisini kaydediyoruz
builder.Services.AddSqlite<HavaDurumuContext>(connectionString);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HavaDurumuContext>();
    db.Database.EnsureCreated(); // Eğer dosya yoksa oluşturur

    // Eğer veritabanı boşsa başlangıç verilerini ekleyelim
    if (!db.Ozetler.Any())
    {
        db.Ozetler.AddRange(
            new HavaDurumuTablosu { Tanim = "Chilly" },
            new HavaDurumuTablosu { Tanim = "Warm" },
            new HavaDurumuTablosu { Tanim = "Hot" }
        );
        db.SaveChanges();
    }
}

builder.Services.AddOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // İŞTE BU YENİ: Scalar arayüzünü açar
}

app.UseHttpsRedirection();

*/

using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore; // Bunu eklemeyi unutma

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVİS KAYITLARI (builder.Build'dan ÖNCE OLMALI) ---
builder.Services.AddSingleton<HavaDurumuMotoru>();
builder.Services.AddOpenApi(); // Bu satır yukarı taşındı!

var connectionString = "Data Source=hava.db";
builder.Services.AddSqlite<HavaDurumuContext>(connectionString);

// --- 2. UYGULAMANIN İNŞA EDİLMESİ ---
var app = builder.Build();

// --- 3. VERİTABANI OLUŞTURMA VE MIDDLEWARE (app.Build'dan SONRA OLMALI) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HavaDurumuContext>();
    db.Database.EnsureCreated();

    if (!db.Ozetler.Any())
    {
        db.Ozetler.AddRange(
            new HavaDurumuTablosu { Tanim = "Chilly" },
            new HavaDurumuTablosu { Tanim = "Warm" },
            new HavaDurumuTablosu { Tanim = "Hot" }
        );
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async (HavaDurumuContext db) =>
{
    // Veritabanındaki tüm özet tanımlarını dizi olarak alıyoruz
    var mevcutOzetler = await db.Ozetler.Select(o => o.Tanim).ToArrayAsync();
    
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            mevcutOzetler[Random.Shared.Next(mevcutOzetler.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/tahmin", async (HavaDurumuContext db) =>
{
    var tumu = await db.Ozetler.ToListAsync();
    if (!tumu.Any()) return Results.NotFound("Veritabanında hiç hava durumu tanımı yok.");

    var rastgele = tumu[Random.Shared.Next(tumu.Count)];
    return Results.Ok(new { Mesaj = "Veritabanından gelen tahmin", Durum = rastgele.Tanim });
});

//yeni map post metodu
app.MapPost("/api/summaries", async (string yeniDurum, HavaDurumuContext db) => 
{
    if (string.IsNullOrWhiteSpace(yeniDurum))
        return Results.BadRequest("Hata: Boş veri gönderilemez.");

    // Veritabanına yeni bir satır ekliyoruz
    var yeniKayit = new HavaDurumuTablosu { Tanim = yeniDurum };
    db.Ozetler.Add(yeniKayit);
    
    // Değişiklikleri diske (hava.db dosyasına) kaydet
    await db.SaveChangesAsync();

    return Results.Created($"/api/summaries", $"Veritabanına eklendi: {yeniDurum}");
});
/*// 3. Yeni Durum Ekleme (POST)
// Not: Tarayıcıdan test edeceksen 'yeniDurum' bilgisini URL'nin sonuna eklemelisin: 
// Örn: /api/summaries?yeniDurum=Mükemmel
app.MapPost("/api/summaries", (string yeniDurum, HavaDurumuMotoru motor) => 
{
    if (string.IsNullOrWhiteSpace(yeniDurum))
        return Results.BadRequest("Hata: Boş veri gönderilemez.");

    motor.OzetEkle(yeniDurum);
    return Results.Created($"/api/summaries", $"Eklendi: {yeniDurum}");
}); */

app.MapDelete("/api/summaries", async (string silinecekDurum, HavaDurumuContext db) => 
{
    // Veritabanında bu metne sahip ilk kaydı bul
    var kayit = await db.Ozetler.FirstOrDefaultAsync(o => o.Tanim == silinecekDurum);

    if (kayit == null)
    {
        return Results.NotFound($"Hata: '{silinecekDurum}' veritabanında bulunamadı.");
    }

    // Kaydı silme kuyruğuna ekle ve değişiklikleri kaydet
    db.Ozetler.Remove(kayit);
    await db.SaveChangesAsync();

    return Results.Ok($"'{silinecekDurum}' veritabanından kalıcı olarak silindi.");
});

// 5. Durum Güncelleme (PUT) - Veritabanı Versiyonu
app.MapPut("/api/summaries", async (string eskiAd, string yeniAd, HavaDurumuContext db) => 
{
    // 1. Validasyon: Yeni isim boş mu?
    if (string.IsNullOrWhiteSpace(yeniAd))
        return Results.BadRequest("Hata: Yeni isim boş olamaz.");

    // 2. Veritabanında eski kaydı bul
    var mevcutKayit = await db.Ozetler.FirstOrDefaultAsync(o => o.Tanim == eskiAd);

    if (mevcutKayit == null)
    {
        return Results.NotFound($"Hata: '{eskiAd}' veritabanında bulunamadı.");
    }

    // 3. Çakışma Kontrolü: Yeni isim zaten veritabanında var mı?
    var varMi = await db.Ozetler.AnyAsync(o => o.Tanim == yeniAd);
    if (varMi)
    {
        return Results.Conflict($"Hata: '{yeniAd}' zaten veritabanında mevcut.");
    }

    // 4. Güncelleme işlemini yap
    mevcutKayit.Tanim = yeniAd;
    
    // 5. Değişiklikleri diske kaydet
    await db.SaveChangesAsync();

    return Results.Ok($"'{eskiAd}' başarıyla '{yeniAd}' olarak güncellendi.");
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}