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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
