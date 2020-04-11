using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class DefaultOutboxDispatcher : IOutboxDispatcher
    {
        public Task Send(object command)
        {
            throw new InvalidOperationException("Outbox dispatcher is not configured. To use dispatching, you can configure it in service.AddOutbox(c => {...})");
        }

        public Task Publish(object evt)
        {
            throw new InvalidOperationException("Outbox dispatcher is not configured. To use dispatching, you can configure it in service.AddOutbox(c => {...})");
        }
    }
}
