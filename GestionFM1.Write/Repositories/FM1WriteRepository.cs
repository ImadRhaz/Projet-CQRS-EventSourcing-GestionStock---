using GestionFM1.Write.Commands;
using GestionFM1.Write.Aggregates;
using GestionFM1.Write.EventStore;
using System;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Repositories
{
    public class FM1WriteRepository
    {
        private readonly IEventStore _eventStore;

        public FM1WriteRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task AddFM1Async(AddFM1Command command)
        {
            var aggregate = new FM1Aggregate();
            var fm1Id = Guid.NewGuid();

            aggregate.AddFM1(
                fm1Id,
                command.CodeSite,
                command.DeviceType,
                command.PsSn,
                command.DateEntre,
                command.ExpirationVerification,
                command.Status,
                command.ExpertId
            );

            foreach (var @event in aggregate.GetChanges())
            {
                await _eventStore.SaveEventAsync(@event);
            }
        }
    }
}