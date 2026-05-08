using ICMS.Application.DTOs.User;

namespace ICMS.Application.DTOs.Auth
{
    public record AuthResponseDto(string AccessToken, string RefreshToken, UserReadDto User);
}
