using System;
using Microsoft.Extensions.DependencyInjection;

namespace Swisschain.Extensions.Idempotency
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
}
