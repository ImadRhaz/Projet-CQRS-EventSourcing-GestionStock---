using System.ComponentModel.DataAnnotations;

namespace GestionFM1.DTOs
{

public class Login2faDTO
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string Email { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required]
    public string TwoFactorCode { get; set; } = string.Empty;
}
}