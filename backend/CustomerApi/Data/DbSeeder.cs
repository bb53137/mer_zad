using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;


namespace CustomerApi.Data;

public static class DbSeeder
{
    private static readonly string[] FirstNames =
    {
        "Luka", "Marko", "Ivan", "Josip", "Danijel",
        "Ema", "Josipa", "Lucija", "Mia", "Marija",
        "Jakov", "Goran", "Ana", "Petra", "Mihael",
        "Sara", "marina", "Nikola", "Tomislav", "Laura"
    };

    private static readonly string[] LastNames =
    {
        "Anić", "Marić", "Horvat", "Valčić", "Jović",
        "Tonković", "Ibrišević", "Vukadin", "Šušnja", "Dukić",
        "Klarić", "Kovačić", "Babić", "Milin", "Morović",
        "Buterin", "Perić", "Knezevic", "Božić", "škibola"
    };

    private static readonly (string City, string Country)[] Locations =
    {
        ("Zagreb", "Hrvatska"),
        ("Split", "Hrvatska"),
        ("Madrid", "Španjolska"),
        ("Barcelona", "Španjolska"),
        ("Berlin", "Njemačka"),
        ("Hamburg", "Njemačka"),
        ("Beč", "Austrija"),
        ("Graz", "Austrija"),
        ("Verona", "Italija"),
        ("Napulj", "Italija"),
        ("Pariz", "Francuska"),
        ("Lyon", "Francuska"),
        ("Budimpešta", " Mađarska"),
        ("Varšava", "Poljska"),
        ("Amsterdam", "Nizozemska"),
        ("Bern", "Švicarska"),
        ("Tirana", "Albanija"),
        ("Beograd", "Srbija"),
        ("Sarajevo", "Bosna i Hercegovina"),
        ("Pogodrica", "Crna Gora")
    };


    private static string NormalizeForEmail(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace(" ", "")
            .Replace("'", "")
            .ToLower();
    }
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Customers.AnyAsync())
            return;

        var customers = new List<Customer>(100000);

        for (int i = 1; i <= 100000; i++)
        {
            var firstName = FirstNames[i % FirstNames.Length];
            var lastName = LastNames[i % LastNames.Length];
            var location = Locations[i % Locations.Length];
            var emailFirstName = NormalizeForEmail(firstName);
            var emailLastName = NormalizeForEmail(lastName);

            customers.Add(new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = $"{emailFirstName}.{emailLastName}.{i}@example.com",
                Phone = i % 4 == 0 ? null : $"+385-91-{100000 + i}",
                City = location.City,
                Country = location.Country,
                IsActive = i % 10 != 0,
                CreatedAt = DateTime.UtcNow.AddMinutes(-i)
            });
        }

        const int batchSize = 5000;

        for (int i = 0; i < customers.Count; i += batchSize)
        {
            var batch = customers.Skip(i).Take(batchSize).ToList();
            await context.Customers.AddRangeAsync(batch);
            await context.SaveChangesAsync();
        }
    }
}