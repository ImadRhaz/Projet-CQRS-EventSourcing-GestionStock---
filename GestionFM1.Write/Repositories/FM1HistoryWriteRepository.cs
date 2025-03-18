using GestionFM1.Write.Commands;
using GestionFM1.Write.Aggregates;
using GestionFM1.Write.EventStore;
using System;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Repositories
{
    public class FM1HistoryWriteRepository
    {
        private readonly IEventStore _eventStore;

        public FM1HistoryWriteRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task AddFM1HistoryAsync(AddFM1HistoryCommand command)
        {
            var aggregate = new FM1HistoryAggregate();
            var fm1HistoryId = Guid.NewGuid();

            aggregate.AddFM1History(fm1HistoryId, command.FM1Id);

            foreach (var @event in aggregate.GetChanges())
            {
                await _eventStore.SaveEventAsync(@event);
            }
        }
    }
}