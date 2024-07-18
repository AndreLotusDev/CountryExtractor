namespace countries_extractor;

public class StateDb
{
    public string id { get; set; }
    public string name { get; set; }
    public string code { get; set; }
    public string country_id { get; set; }
    public string created_by_user_id { get; set; }
    public DateTime created_at { get; set; }
    public object update_at { get; set; }
    public object update_by_user_id { get; set; }
    public string status { get; set; }
}