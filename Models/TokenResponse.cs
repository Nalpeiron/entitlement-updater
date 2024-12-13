using System.Text.Json.Serialization;

namespace EntitlementUpdater.Models;

public sealed class TokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
}