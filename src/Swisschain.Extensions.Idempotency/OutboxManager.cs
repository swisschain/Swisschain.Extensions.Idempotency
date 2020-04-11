using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency.Outbox
{
    internal sealed class OutboxManager : IOutboxManager
    {
        private readonly IOutboxDispatcher _dispatcher;
        private readonly IOutboxRepository _repository;

        public OutboxManager(IOutboxDispatcher dispatcher,
            IOutboxRepository repository)
        {
            _dispatcher = dispatcher;
            _repository = repository;
        }

        public async Task<Outbox> Open(string requestId, Func<Task<long>> aggregateIdFactory)
        {
            return await _repository.Open(requestId, aggregateIdFactory);
        }

        public async Task Store(Outbox outbox)
        {
            await _repository.Save(outbox, OutboxPersistingReason.Storing);
        }

        public async Task EnsureDispatched(Outbox outbox)
        {
            if (outbox.IsDispatched)
            {
                return;
            }

            foreach (var command in outbox.Commands)
            {
                await _dispatcher.Send(command);
            }

            foreach (var evt in outbox.Events)
            {
                await _dispatcher.Publish(evt);
            }

            await _repository.Save(outbox, OutboxPersistingReason.Dispatching);
        }

        public async Task EnsureDispatched(Outbox outbox, IOutboxDispatcher dispatcher)
        {
            if (outbox.IsDispatched)
            {
                return;
            }

            foreach (var command in outbox.Commands)
            {
                await dispatcher.Send(command);
            }

            foreach (var evt in outbox.Events)
            {
                await dispatcher.Publish(evt);
            }

            await _repository.Save(outbox, OutboxPersistingReason.Dispatching);
        }
    }
}