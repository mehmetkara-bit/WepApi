public class HavaDurumuMotoru
{
    // Veriler artık burada bir Liste (List) olarak tutuluyor
    private List<string> _summaries = new()
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public string RastgeleHavaDurumuGetir()
    {
        return _summaries[Random.Shared.Next(_summaries.Count)];
    }

    public void OzetEkle(string yeniOzet)
    {
        if (!_summaries.Contains(yeniOzet)) // Aynı veri varsa ekleme dedik
        {
            _summaries.Add(yeniOzet);
        }
    }

    public bool OzetSil(string silinecekOzet)
{
    // List.Remove metodu eğer öğeyi bulup silerse 'true', bulamazsa 'false' döner.
    return _summaries.Remove(silinecekOzet);
}

    // Bu metod, /weatherforecast için lazım olacak
    public string[] TumunuGetir() => _summaries.ToArray();
}