using System.Threading.Tasks;

namespace GestionFM1.Core.Interfaces;

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    Task Handle(TEvent @event);
}