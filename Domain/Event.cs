namespace Domain;

public class Event
{
    public string EventName { get; set; }
    public string Action { get; set; }
    public Repository Repository { get; set; }
    public Team Team { get; set; }
}