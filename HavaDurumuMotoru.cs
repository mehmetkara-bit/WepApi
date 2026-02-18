public class HavaDurumuMotoru
{
    // Hava durumu listemiz 
    private static string[] summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static string RastgeleHavaDurumuGetir()
    {
        // 1. Bir 'zar' oluşturuyoruz
        Random zar = new Random();

        // 2. Dizinin uzunluğuna göre rastgele bir sayı (indeks) seçiyoruz
        // summaries.Length bize dizide kaç eleman olduğunu verir (Burada 10)
        // Next(0, 10) -> 0 dahil, 10 hariç bir sayı üretir (0,1,2...9)
        int rastgeleIndeks = zar.Next(0, summaries.Length);

        // 3. Seçilen indeksteki kelimeyi geri gönderiyoruz
        return summaries[rastgeleIndeks];
    }
}