namespace Cfo.Cats.Application.Common.Security;

public class ResetPasswordFormModel
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}