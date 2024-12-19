using System.Text.Json;
using System.Text.Json.Serialization;

namespace ET.Api.Services;

public class IpService(HttpClient httpClient) : IIpService
{
    private const string N = "not available";
    public async Task<string> GetIpCountryCode(string ipAddress)
    {
        var response = await httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}");
        if (!response.IsSuccessStatusCode) return N;
        
        var content = await response.Content.ReadAsStringAsync();
        var ip = JsonSerializer.Deserialize<Ip>(content);

        return ip?.CountryCode ?? N;
    }

    private class Ip
    {
        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; init; }
    }
}