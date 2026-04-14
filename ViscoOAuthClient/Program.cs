using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var options = ClientOptions.FromEnvironment();

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(options.ApiBaseUrl)
};

httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", options.BearerToken);

var payload = JsonSerializer.Serialize(new { key1 = options.Key1 });
using var content = new StringContent(payload, Encoding.UTF8, "application/json");

Console.WriteLine($"Calling {httpClient.BaseAddress}api/xml ...");
using var response = await httpClient.PostAsync("api/xml", content);
var body = await response.Content.ReadAsStringAsync();

if (!response.IsSuccessStatusCode)
{
    Console.Error.WriteLine($"Request failed: {(int)response.StatusCode} {response.StatusCode}");
    Console.Error.WriteLine(body);
    return;
}

Console.WriteLine("Success. XML response:");
Console.WriteLine(body);

sealed record ClientOptions(string ApiBaseUrl, string BearerToken, string Key1)
{
    public static ClientOptions FromEnvironment()
    {
        var apiBaseUrl = Environment.GetEnvironmentVariable("VISCO_API_BASE_URL")
            ?? "http://localhost:5000/";

        var bearerToken = Environment.GetEnvironmentVariable("VISCO_BEARER_TOKEN");
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            throw new InvalidOperationException(
                "Set VISCO_BEARER_TOKEN environment variable with a valid OAuth/JWT access token.");
        }

        var key1 = Environment.GetEnvironmentVariable("VISCO_KEY1") ?? "sample-key-123";

        if (!apiBaseUrl.EndsWith('/'))
        {
            apiBaseUrl += '/';
        }

        return new ClientOptions(apiBaseUrl, bearerToken, key1);
    }
}
