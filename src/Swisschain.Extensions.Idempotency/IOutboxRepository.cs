using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IOutboxRepository
    {
        Task<Outbox> Open(string requestId, Func<Task<long>> aggregateIdFactory);
        Task<Outbox> Open(string requestId);
        Task Save(Outbox outbox, OutboxPersistingReason reason);
    }
}
