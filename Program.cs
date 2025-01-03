using System.Net.Http.Json;
using EntitlementUpdater.Models;

namespace EntitlementUpdater;

static class Program
{
    private static readonly HttpClient Client = new();
    private const string BaseUrl = "https://api.example.com"; // Replace with actual base URL
    private const string ClientId = "your_client_id"; // Replace with actual client ID
    private const string ClientSecret = "your_client_secret"; // Replace with actual client secret
    private const string TokenUrl = "https://auth.example.com/protocol/openid-connect/token"; // Replace with actual token URL
    private const string TenantId = "your_tenant_id"; // Replace with actual tenant ID

    static async Task Main()
    {
        var token = await GetOpenIdTokenAsync();
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        Client.DefaultRequestHeaders.Add("N-TenantId", TenantId);

        // Get the first 100 entitlements
        var entitlements = await GetEntitlementsAsync();

        // Update each entitlement
        foreach (var entitlement in entitlements)
        {
            await UpdateEntitlementAsync(entitlement.Id);
        }
    }

    private static async Task<string> GetOpenIdTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, TokenUrl)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", ClientId },
                { "client_secret", ClientSecret }
            })
        };

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Failed to retrieve access token");
    }

    private static async Task<List<Entitlement>> GetEntitlementsAsync()
    {
        var response = await Client.GetFromJsonAsync<EntitlementListResponse>($"{BaseUrl}/api/v1/entitlements?pageSize=100&pageNumber=1");
        return response?.Items ?? new List<Entitlement>();
    }

    private static async Task UpdateEntitlementAsync(string entitlementId)
    {
        var updateRequest = new UpdateEntitlementRequest
        {
            LeasePeriod = new IntervalType
            {
                Count = 52,
                Type = "week"
            }
        };

        var response = await Client.PatchAsJsonAsync($"{BaseUrl}/api/v1/entitlements/{entitlementId}", updateRequest);
        response.EnsureSuccessStatusCode();
        Console.WriteLine($"Entitlement {entitlementId} updated");
    }
}