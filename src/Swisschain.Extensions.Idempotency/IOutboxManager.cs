using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IOutboxManager
    {
        Task<Outbox> Open(string requestId, Func<Task<long>> aggregateIdFactory);
        Task<Outbox> Open(string requestId);
        Task Store(Outbox outbox);
        Task EnsureDispatched(Outbox outbox);
        Task EnsureDispatched(Outbox outbox, IOutboxDispatcher dispatcher);
    }
}
