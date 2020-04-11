using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Swisschain.Extensions.Idempotency.Outbox
{
    public static class OutboxServiceCollectionExtensions
    {
        public static IServiceCollection AddOutbox(this IServiceCollection services, Action<OutboxConfigurationBuilder> config)
        {
            services.AddTransient<IOutboxManager, OutboxManager>();

            if (config != null)
            {
                var configBuilder = new OutboxConfigurationBuilder(services);

                config.Invoke(configBuilder);
            }
            else
            {

            }

            return services;
        }
    }

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


    public sealed class OutboxConfigurationBuilder
    {
        internal OutboxConfigurationBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
