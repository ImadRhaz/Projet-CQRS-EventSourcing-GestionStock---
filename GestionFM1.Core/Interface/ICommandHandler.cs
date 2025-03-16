using System.Threading.Tasks;

namespace GestionFM1.Core.Interfaces;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task Handle(TCommand command);
}