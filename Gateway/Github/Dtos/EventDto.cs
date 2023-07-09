namespace Gateway.Github.Dtos;

public class EventDto
{
    public string EventName { get; set; }
    public string Action { get; set; }
    public RepositoryDto Repository { get; set; }
    public TeamDto Team { get; set; }


}