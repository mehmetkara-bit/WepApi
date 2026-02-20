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

    public bool OzetGuncelle(string eskiOzet, string yeniOzet)
    {
    // Önce listenin içinde bu metnin hangi sırada (indekste) olduğunu buluyoruz
    int indeks = _summaries.IndexOf(eskiOzet);

    // Eğer metin listede yoksa IndexOf -1 değerini döndürür
    if (indeks == -1)
    {
        return false; // Öğe bulunamadı, işlem başarısız
    }

    // Eğer yeni değer zaten listede varsa (tekrarı önlemek için)
    if (_summaries.Contains(yeniOzet))
    {
        return false; 
    }

    // Bulduğumuz sıradaki eski veriyi yeni veriyle değiştiriyoruz
    _summaries[indeks] = yeniOzet;
    return true;
    }  



    // Bu metod, /weatherforecast için lazım olacak
    public string[] TumunuGetir() => _summaries.ToArray();
}