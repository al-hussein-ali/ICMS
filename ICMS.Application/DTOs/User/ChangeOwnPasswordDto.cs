namespace ICMS.Application.DTOs.User
{
    public record ChangeOwnPasswordDto(string OldPassword, string NewPassword, string ConfirmPassword);
}
