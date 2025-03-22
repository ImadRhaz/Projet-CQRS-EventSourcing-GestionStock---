using System.ComponentModel.DataAnnotations;

// CommandeUpdateModel.cs (or wherever you put your DTOs)
namespace GestionFM1.Core.Models
{
    public class CommandeUpdateModel
    {
        [Required] // Enforce that EtatCommande must be provided
        public string EtatCommande { get; set; } = string.Empty;
    }
}