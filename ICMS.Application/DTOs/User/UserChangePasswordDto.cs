namespace ICMS.Application.DTOs.User
{
    public record UserChangePasswordDto(string NewPassword, string ConfirmPassword);
}
