using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BarnameNevis1401.Models;

public partial class Occasion
{
    [JsonPropertyName("is_holiday")]
    [JsonProperty("is_holiday")]
    public bool IsHoliday { get; set; }

    [JsonProperty("events")]
    public Event[] Events { get; set; }
}

public partial class Event
{
    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("additional_description")]
    public string AdditionalDescription { get; set; }

    [JsonPropertyName("is_religious")]
    [JsonProperty("is_religious")]
    public bool IsReligious { get; set; }
}