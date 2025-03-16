using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GestionFM1.Core.Models;

public class User : IdentityUser
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
}