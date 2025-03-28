using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Commands;

public class UpdateCommandeEtatCommand
{
    public int CommandeId { get; set; }
    public string EtatCommande { get; set; }
}