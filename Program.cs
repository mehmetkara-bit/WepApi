using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Hizmeti sisteme kaydediyoruz
builder.Services.AddSingleton<HavaDurumuMotoru>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // İŞTE BU YENİ: Scalar arayüzünü açar
}

app.UseHttpsRedirection();

// 1. Standart Liste (Motoru enjekte ettik)
app.MapGet("/weatherforecast", (HavaDurumuMotoru motor) =>
{
    var mevcutOzetler = motor.TumunuGetir();
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

// 2. Tekli Tahmin
app.MapGet("/tahmin", (HavaDurumuMotoru motor) =>
{
    return new { Mesaj = "Hizmetten gelen tahmin", Durum = motor.RastgeleHavaDurumuGetir() };
});

// 3. Yeni Durum Ekleme (POST)
// Not: Tarayıcıdan test edeceksen 'yeniDurum' bilgisini URL'nin sonuna eklemelisin: 
// Örn: /api/summaries?yeniDurum=Mükemmel
app.MapPost("/api/summaries", (string yeniDurum, HavaDurumuMotoru motor) => 
{
    if (string.IsNullOrWhiteSpace(yeniDurum))
        return Results.BadRequest("Hata: Boş veri gönderilemez.");

    motor.OzetEkle(yeniDurum);
    return Results.Created($"/api/summaries", $"Eklendi: {yeniDurum}");
});

// 4. Durum Silme (DELETE)
app.MapDelete("/api/summaries", (string silinecekDurum, HavaDurumuMotoru motor) => 
{
    // Motor içindeki silme metodunu çağırıyoruz
    bool sonuc = motor.OzetSil(silinecekDurum);

    if (sonuc)
    {
        // Öğe bulundu ve silindiyse 200 OK döner
        return Results.Ok($"'{silinecekDurum}' listeden başarıyla silindi.");
    }
    else
    {
        // Öğe listede yoksa 404 Not Found döner
        return Results.NotFound($"Hata: '{silinecekDurum}' listede bulunamadı.");
    }
});
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}