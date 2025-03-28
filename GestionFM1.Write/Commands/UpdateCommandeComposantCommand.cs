using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Commands;



public class UpdateCommandeComposantCommand
{
    public int CommandeId { get; set; }
    public Guid NewComposentId { get; set; }
}