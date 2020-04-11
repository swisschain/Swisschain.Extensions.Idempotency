using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency.Outbox
{
    public interface IOutboxManager
    {
        Task<Outbox> Open(string requestId, Func<Task<long>> aggregateIdFactory);
        Task Store(Outbox outbox);
        Task EnsureDispatched(Outbox outbox);
        Task EnsureDispatched(Outbox outbox, IOutboxDispatcher dispatcher);
    }
}