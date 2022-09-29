using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;

namespace IAC.Bot.Weather
{
    public sealed class WeatherForecast
    {
        private static readonly string baseAddress = "https://api.openweathermap.org/data/2.5/";
        private static readonly string appId = "appid=" + Environment.GetEnvironmentVariable("WEATHER_API_KEY");

        private readonly HttpClient _httpClient;

        #region Singleton implementation
        private WeatherForecast()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress, UriKind.Absolute),
            };
        }

        private static WeatherForecast _instance;

        public static WeatherForecast Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WeatherForecast();
                }

                return _instance;
            }
        }
        #endregion

        public async Task<Weather> GetByCityAsync(string city)
        {
            var response = await _httpClient.GetAsync($"weather?q={city}&units=metric&{appId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("City not found.");
            }

            using var rs = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };


            Weather result = await JsonSerializer.DeserializeAsync<Weather>(rs, options);

            return result;
        }
    }
}
