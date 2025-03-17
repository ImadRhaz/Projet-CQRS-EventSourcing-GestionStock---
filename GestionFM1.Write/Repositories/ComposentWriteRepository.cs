using GestionFM1.Write.Commands;
using GestionFM1.Write.Aggregates;
using GestionFM1.Write.EventStore;
using System;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Repositories
{
    public class ComposentWriteRepository
    {
        private readonly IEventStore _eventStore;

        public ComposentWriteRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task AddComposentAsync(AddComposentCommand command)
        {
            var aggregate = new ComposentAggregate();
            var composentId = Guid.NewGuid();

            aggregate.AddComposent(
                composentId,
                command.ItemBaseId,
                command.ProductName,
                command.SN,
                command.TotalAvailable,
                command.UrgentOrNot,
                command.OrderOrNot,
                command.FM1Id
            );

            foreach (var @event in aggregate.GetChanges())
            {
                await _eventStore.SaveEventAsync(@event);
            }
        }
    }
}