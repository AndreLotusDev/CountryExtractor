using Newtonsoft.Json;

namespace countries_extractor;

public static class CityExtractor
{
    public static void Run()
    {
        var statesInDBFile = "./states_in_db_stg.json";
        var citiesJson = "./cities.json";

        var statesInDb = JsonConvert.DeserializeObject<List<StateDb>>(File.ReadAllText(statesInDBFile));
        var cities = JsonConvert.DeserializeObject<List<City>>(File.ReadAllText(citiesJson));

        var sqlInsertTemplate =
            "INSERT INTO city (id, \"name\", state_id, created_by_user_id, created_at, status) VALUES";

        List<string> commands = new();
        commands.Add(sqlInsertTemplate);
        foreach (var city in cities)
        {
            var stateFound = statesInDb.FirstOrDefault(f => f.code == city.state_code);

            if (stateFound is null)
                continue;

            var stateId = stateFound.id;

            var sqlTemp =
                $"('{Guid.NewGuid()}', '{city.name.Replace("'", "''")}', '{stateId}', '-1', now(), 'ACTIVE'),";
            commands.Add($"{sqlTemp}");
        }

        //Pick the last row and remove the last ,
        var lastRow = commands.Last();
        commands.Remove(lastRow);
        int lastIndex = lastRow.LastIndexOf(',');
        if (lastIndex >= 0)
        {
            lastRow = lastRow.Remove(lastIndex, 1);
        }

        commands.Add(lastRow);

        var finalSql = " ON CONFLICT (id) DO NOTHING;";
        commands.Add(finalSql);

        //Dump into desktop in a file
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var file = Path.Combine(desktop, "insert_cities.sql");
        File.WriteAllLines(file, commands);
    }
}