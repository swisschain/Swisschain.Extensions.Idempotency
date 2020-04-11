using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class DefaultOutboxRepository : IOutboxRepository 
    {
        public Task<Outbox> Open(string requestId, Func<Task<long>> aggregateIdFactory)
        {
            throw new InvalidOperationException("Outbox repository is not configured. To use outbox, you need to configure repository in service.AddOutbox(c => {...})");
        }

        public Task Save(Outbox outbox, OutboxPersistingReason reason)
        {
            throw new InvalidOperationException("Outbox repository is not configured. To use outbox, you need to configure repository in service.AddOutbox(c => {...})");
        }
    }
}