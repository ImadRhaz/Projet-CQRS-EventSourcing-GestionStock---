using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Commands;

public class DecrementComposantStockCommand
{
    public Guid ComposentId { get; set; }
}