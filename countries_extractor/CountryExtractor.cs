using System.Text.Json;
using countries_extractor;

namespace countries_extractor;

public static class CountryExtractor
{
    public static void Run()
    {
        var countriesJson = "./countries.json";
        var countriesInDbJson = "./countries_in_db.json";

        var countries = JsonSerializer.Deserialize<List<Country>>(File.ReadAllText(countriesJson));
        var countriesInDb = JsonSerializer.Deserialize<List<CountryDb>>(File.ReadAllText(countriesInDbJson));

        var dontHaveIsoCode3 = countriesInDb.Where(w => w.alpha_3.Trim() == string.Empty).ToList(); //23

        var countriesThatDoesntExistInOurDb =
            countries.Where(w => !countriesInDb.Any(a => a.alpha_3.Trim() == w.iso3.Trim())).ToList();

        var insertTemplate =
            "INSERT INTO country (id, country_name, international_dialing, created_by_user_id, created_at, status, alpha_2, alpha_3) VALUES";

        List<string> commands = new();

        foreach (var countryToInsert in countriesThatDoesntExistInOurDb)
        {
            var sqlTemp =
                $"{insertTemplate} ('{Guid.NewGuid()}', '{countryToInsert.name}', 'N/A', '-1', now(), 'ACTIVE', '{countryToInsert.iso2}', '{countryToInsert.iso3}');";
            commands.Add(sqlTemp);
        }

        //Dump into a file in desktop
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var file = Path.Combine(desktop, "insert_countries.sql");
        File.WriteAllLines(file, commands);
    }
}