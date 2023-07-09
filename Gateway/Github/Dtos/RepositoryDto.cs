using Domain.Converters;
using Newtonsoft.Json;

namespace Gateway.Github.Dtos;

public class RepositoryDto
{
    public string Id { get; set; }
    public string Name { get;set; }
    [JsonConverter(typeof(EpochJsonConverter))]
    public DateTime CreatedAt { get; set; }
    [JsonConverter(typeof(EpochJsonConverter))]
    public DateTime PushedAt { get; set; }
}