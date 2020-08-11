using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class DefaultOutboxRepository : IOutboxRepository 
    {
        public Task<Outbox> GetOrDefault(IUnitOfWork unitOfWork, IOutboxDispatcher dispatcher, string idempotencyId)
        {
            throw new InvalidOperationException("Outbox repository is not configured. To use outbox, you need to configure a persistence in service.AddIdempotency(c => {...})");
        }

        public Task Add(Outbox outbox)
        {
            throw new InvalidOperationException("Outbox repository is not configured. To use outbox, you need to configure a persistence in service.AddIdempotency(c => {...})");
        }

        public Task Update(Outbox outbox)
        {
            throw new InvalidOperationException("Outbox repository is not configured. To use outbox, you need to configure a persistence in service.AddIdempotency(c => {...})");
        }
    }
}
