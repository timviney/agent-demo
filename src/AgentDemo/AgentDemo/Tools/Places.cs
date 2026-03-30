using System.ComponentModel;

namespace AgentDemo.Tools;

public class Places
{
    private static readonly HttpClient Http = new();

    [Description("Search for the top 10 places matching a name and return raw JSON results. This contains information such as id to use for more specific weather queries, as well as country, timezone, and coordinates.")]
    public static async Task<string> SearchPlaces([Description("The name of the location to search for. Include only the name of the of the settlement, no other identifiers")] string locationName)
    {
        TerminalUi.Current?.LogToolUse(nameof(SearchPlaces), (nameof(locationName), locationName));

        if (string.IsNullOrWhiteSpace(locationName))
        {
            return "{\"error\":\"Please provide a place name.\"}";
        }

        try
        {
            var url =
                $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(locationName)}&count=10&language=en&format=json";

            // Return provider JSON directly so the model can parse full structured results.
            var json = await Http.GetStringAsync(url).ConfigureAwait(false);
            
            return json;
        }
        catch
        {
            return "{\"error\":\"Places service is currently unavailable. Please try again in a moment.\"}";
        }
    }
}
