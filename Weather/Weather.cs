using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IAC.Bot.Weather
{
    public sealed class Weather
    {
        public Coordinates Coord { get; set; }
        [JsonPropertyName("weather")]
        public IEnumerable<WeatherDeatils> WeatherDetails { get; set; }
        public string Base { get; set; }
        public int Visibility { get; set; }
        public Temperature Main { get; set; }
    }
}
