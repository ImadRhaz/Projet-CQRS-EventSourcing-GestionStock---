using System.ComponentModel.DataAnnotations;

namespace GestionFM1.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Ajout de l'initialisation
        
        [Required]
        public string Password { get; set; } = string.Empty; // Ajout de l'initialisation
    }
}