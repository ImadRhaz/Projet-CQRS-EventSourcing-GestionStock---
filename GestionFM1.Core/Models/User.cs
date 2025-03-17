using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using GestionFM1.Core.Models;

namespace GestionFM1.Core.Models;

public class User : IdentityUser
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public List<FM1>? FM1s { get; set; }
    public List<Commande>? Commandes { get; set; }
}