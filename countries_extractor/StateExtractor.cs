using Newtonsoft.Json;

namespace countries_extractor;

public static class StateExtractor
{
    public static void Run()
    {
        var countriesInDbJson = "./countries_in_db_stg.json";
        var statesJson = "./states.json";
        
        var countriesInDb = JsonConvert.DeserializeObject<List<CountryDb>>(File.ReadAllText(countriesInDbJson));
        var states = JsonConvert.DeserializeObject<List<State>>(File.ReadAllText(statesJson));

        var sqlInsertTemplate = 
            "INSERT INTO state (id, \"name\", code, country_id, created_by_user_id, created_at, status) VALUES";

        List<string> commands = new();
        commands.Add(sqlInsertTemplate);
        foreach (var state in states)
        {
            var countryFound = countriesInDb.FirstOrDefault(f => f.alpha_2 == state.country_code);

            if (countryFound is null)
                continue;
            
            var countryId = countryFound.id;

            var sqlTemp = $"('{Guid.NewGuid()}', '{state.name.Replace("'", "''")}', '{state.state_code}', '{countryId}', '-1', now(), 'ACTIVE'),";
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
        var file = Path.Combine(desktop, "insert_states.sql");
        File.WriteAllLines(file, commands);
    }
}