using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class DefaultOutboxDispatcher : IOutboxDispatcher
    {
        public Task Send(object command)
        {
            throw new InvalidOperationException("Outbox dispatcher is not configured. To use dispatching, you can configure messaging in service.AddIdempotency(c => {...})");
        }

        public Task Publish(object evt)
        {
            throw new InvalidOperationException("Outbox dispatcher is not configured. To use dispatching, you can configure messaging in service.AddIdempotency(c => {...})");
        }
    }
}
