using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domain.Converters;

public class EpochJsonConverter : JsonConverter
{
    private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var jObject = new JObject();
        jObject["$date"] = new DateTimeOffset((DateTime)value).ToUnixTimeMilliseconds();
        jObject.WriteTo(writer);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return reader.Value switch
        {
            null => null,
            DateTime => reader.Value,
            _ => _epoch.AddSeconds((long)reader.Value)
        };
    }

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType) => objectType == typeof(DateTime);
}