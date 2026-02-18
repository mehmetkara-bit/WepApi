public class HavaDurumuMotoru
{
    private string[] summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    // 'static' takısını kaldırdık
    public string RastgeleHavaDurumuGetir()
    {
        return summaries[Random.Shared.Next(summaries.Length)];
    }
}