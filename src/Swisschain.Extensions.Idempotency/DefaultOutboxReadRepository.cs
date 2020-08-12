using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class DefaultOutboxReadRepository : IOutboxReadRepository
    {
        public Task<Outbox> GetOrDefault(string idempotencyId)
        {
            throw new InvalidOperationException("Outbox repository is not configured. To use outbox, you need to configure a persistence in service.AddIdempotency(c => {...})");
        }
    }
}
