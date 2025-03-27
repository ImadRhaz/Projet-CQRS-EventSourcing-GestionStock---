using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionFM1.Core.Interfaces;

public interface IEventStore
{
    Task SaveEventAsync(IEvent @event);
    Task<IEnumerable<IEvent>> LoadEventsAsync(string aggregateId);
}