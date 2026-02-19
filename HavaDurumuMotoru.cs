public class HavaDurumuMotoru
{
    // Array yerine List kullanıyoruz ki Add() diyebilelim
    private List<string> summaries = new List<string>
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild"
    };

    public string RastgeleHavaDurumuGetir()
    {
        return summaries[Random.Shared.Next(summaries.Count)];
    }

    // YENİ: Dışarıdan yeni bir özet ekleme metodu
    public void OzetEkle(string yeniOzet)
    {
        summaries.Add(yeniOzet);
    }

    // Mevcut listeyi görmek için
    public List<string> TumunuGetir() => summaries;
}