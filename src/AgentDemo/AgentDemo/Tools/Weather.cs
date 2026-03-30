using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgentDemo.Tools;

public class Weather
{
    private static readonly HttpClient Http = new();

    [Description("Get the weather for a given location. This returns the largest known place, so search via ID to be more specific. Remember that users may provide ambiguous location names, so you should decide what to search for based on the User's location")]
    public static async Task<string> GetWeather([Description("The location name to get the weather for. Include only the name of the of the settlement, no other identifiers")] string location)
    {
        TerminalUi.Current?.LogToolUse(nameof(GetWeather), (nameof(location), location));

        if (string.IsNullOrWhiteSpace(location))
        {
            return "Please provide a location.";
        }

        try
        {
            var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(location)}&count=1&language=en&format=json";

            var geoJson = await Http.GetStringAsync(geoUrl).ConfigureAwait(false);
            var geo = JsonSerializer.Deserialize(geoJson, WeatherJsonContext.Default.GeocodingResponse);
            var place = geo?.Results is { Length: > 0 } ? geo.Results[0] : null;

            if (place is null)
            {
                return $"I couldn't find a place matching '{location}'.";
            }

            var weatherUrl =
                $"https://api.open-meteo.com/v1/forecast?latitude={place.Latitude.ToString(CultureInfo.InvariantCulture)}&longitude={place.Longitude.ToString(CultureInfo.InvariantCulture)}&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m&timezone=auto";

            var weatherJson = await Http.GetStringAsync(weatherUrl).ConfigureAwait(false);
            var weather = JsonSerializer.Deserialize(weatherJson, WeatherJsonContext.Default.WeatherResponse);
            var current = weather?.Current;

            if (current is null)
            {
                return $"I found {place.Name}, but couldn't load current weather right now.";
            }

            var placeName = FormatPlaceName(place);

            return
                $"Current weather for {placeName} at {current.Time}: {DescribeCode(current.WeatherCode)}, {current.Temperature2M:F1}°C (feels like {current.ApparentTemperature:F1}°C), humidity {current.RelativeHumidity2M}%, wind {current.WindSpeed10M:F1} km/h.";
        }
        catch
        {
            return "Weather service is currently unavailable. Please try again in a moment.";
        }
    }
    
    [Description("Get the weather for a given location id.")]
    public static async Task<string> GetWeatherById([Description("The location id to get the weather")] string locationId)
    {
        TerminalUi.Current?.LogToolUse(nameof(GetWeatherById), (nameof(locationId), locationId));

        if (string.IsNullOrWhiteSpace(locationId))
        {
            return "Please provide a location ID.";
        }

        try
        {
            var geoUrl =
                $"https://geocoding-api.open-meteo.com/v1/get?id={Uri.EscapeDataString(locationId)}&language=en&format=json";

            var geoJson = await Http.GetStringAsync(geoUrl).ConfigureAwait(false);
            var place = JsonSerializer.Deserialize(geoJson, WeatherJsonContext.Default.GeocodingResult);

            if (place is null)
            {
                return $"I couldn't find a place matching location ID '{locationId}'.";
            }

            var weatherUrl =
                $"https://api.open-meteo.com/v1/forecast?latitude={place.Latitude.ToString(CultureInfo.InvariantCulture)}&longitude={place.Longitude.ToString(CultureInfo.InvariantCulture)}&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m&timezone=auto";

            var weatherJson = await Http.GetStringAsync(weatherUrl).ConfigureAwait(false);
            var weather = JsonSerializer.Deserialize(weatherJson, WeatherJsonContext.Default.WeatherResponse);
            var current = weather?.Current;

            if (current is null)
            {
                return $"I found {place.Name}, but couldn't load current weather right now.";
            }

            var placeName = FormatPlaceName(place);

            return
                $"Current weather for {placeName} at {current.Time}: {DescribeCode(current.WeatherCode)}, {current.Temperature2M:F1}°C (feels like {current.ApparentTemperature:F1}°C), humidity {current.RelativeHumidity2M}%, wind {current.WindSpeed10M:F1} km/h.";
        }
        catch
        {
            return "Weather service is currently unavailable. Please try again in a moment.";
        }
    }


    private static string DescribeCode(int code) => code switch
    {
        0 => "clear sky",
        1 => "mostly clear",
        2 => "partly cloudy",
        3 => "overcast",
        45 or 48 => "foggy",
        51 or 53 or 55 => "drizzle",
        61 or 63 or 65 => "rain",
        71 or 73 or 75 => "snow",
        80 or 81 or 82 => "rain showers",
        95 => "thunderstorm",
        _ => "mixed conditions"
    };

    private static string FormatPlaceName(GeocodingResult place)
    {
        var parts = new[] { place.Name, place.Admin1, place.Country }
            .Where(static p => !string.IsNullOrWhiteSpace(p));

        return string.Join(", ", parts);
    }

    public sealed record GeocodingResponse(
        [property: JsonPropertyName("results")]
        GeocodingResult[]? Results);

    public sealed record GeocodingResult(
        [property: JsonPropertyName("name")]
        string Name,
        [property: JsonPropertyName("country")]
        string? Country,
        [property: JsonPropertyName("admin1")]
        string? Admin1,
        [property: JsonPropertyName("latitude")]
        double Latitude,
        [property: JsonPropertyName("longitude")]
        double Longitude);

    public sealed record WeatherResponse(
        [property: JsonPropertyName("current")]
        CurrentWeather? Current);

    public sealed record CurrentWeather(
        [property: JsonPropertyName("time")]
        string Time,
        [property: JsonPropertyName("interval")]
        int Interval,
        [property: JsonPropertyName("temperature_2m")]
        double Temperature2M,
        [property: JsonPropertyName("relative_humidity_2m")]
        int RelativeHumidity2M,
        [property: JsonPropertyName("apparent_temperature")]
        double ApparentTemperature,
        [property: JsonPropertyName("weather_code")]
        int WeatherCode,
        [property: JsonPropertyName("wind_speed_10m")]
        double WindSpeed10M);
}

[JsonSerializable(typeof(Weather.GeocodingResponse))]
[JsonSerializable(typeof(Weather.WeatherResponse))]
internal partial class WeatherJsonContext : JsonSerializerContext;
