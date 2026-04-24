using System.Text.Json.Serialization;

namespace ICMS.Application.DTOs.Auth
{
    public record RefreshTokenRequestDto(
        [property: JsonPropertyName("RefreshToken")] string RefreshToken,
        [property: JsonPropertyName("AccessToken")] string? AccessToken = null);
}
