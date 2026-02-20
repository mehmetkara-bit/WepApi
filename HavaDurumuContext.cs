using Microsoft.EntityFrameworkCore;

// Bu sınıf veritabanı ile C# arasındaki köprüdür
public class HavaDurumuContext : DbContext
{
    public HavaDurumuContext(DbContextOptions<HavaDurumuContext> options) : base(options) { }

    // Bu, veritabanındaki "Ozetler" tablosunu temsil eder
    public DbSet<HavaDurumuTablosu> Ozetler { get; set; }
}

// Veritabanındaki tablonun yapısı
public class HavaDurumuTablosu
{
    public int Id { get; set; } // Benzersiz kimlik (Primary Key)
    public string Tanim { get; set; } = string.Empty; // "Hot", "Cold" vb.
}