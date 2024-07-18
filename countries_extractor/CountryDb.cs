namespace countries_extractor;

public class CountryDb
{
    public string id { get; set; }
    public string country_name { get; set; }
    public string international_dialing { get; set; }
    public string created_by_user_id { get; set; }
    public DateTimeOffset created_at { get; set; }
    public object update_at { get; set; }
    public object update_by_user_id { get; set; }
    public string status { get; set; }
    public string alpha_2 { get; set; }
    public string alpha_3 { get; set; }
}