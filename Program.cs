var builder = WebApplication.CreateBuilder(args);

// "Sistemde tek bir tane HavaDurumuMotoru olsun ve herkes onu kullansın" diyoruz.
builder.Services.AddSingleton<HavaDurumuMotoru>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Buraya dikkat: Parametre olarak (HavaDurumuMotoru motor) ekledik!
app.MapGet("/tahmin", (HavaDurumuMotoru motor) =>
{
    var sonuc = motor.RastgeleHavaDurumuGetir();
    return new { Mesaj = "Hizmetten gelen tahmin", Durum = sonuc };
});

// POST: Dışarıdan veri gönderirken MapPost kullanılır
app.MapPost("/api/summaries", (string yeniDurum, HavaDurumuMotoru motor) => 
{
    if (string.IsNullOrWhiteSpace(yeniDurum))
    {
        return Results.BadRequest("Hava durumu boş olamaz!");
    }

    motor.OzetEkle(yeniDurum);

    // 201 Created: "İstediğin şeyi başarıyla oluşturdum" demek
    return Results.Created($"/api/summaries", $"Yeni durum eklendi: {yeniDurum}");
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
