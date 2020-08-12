using System;
using Microsoft.Extensions.DependencyInjection;

namespace Swisschain.Extensions.Idempotency
{
    public static class IdempotencyServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddIdempotency(this IServiceCollection services, Action<IdempotencyConfigurationBuilder> config)
        {
            services.AddTransient<IOutboxManager, OutboxManager>();
            services.AddTransient<IIdGenerator, IdGenerator>();

            services.AddTransient<IOutboxDispatcher, DefaultOutboxDispatcher>();
            services.AddTransient<IOutboxReadRepository, DefaultOutboxReadRepository>();
            services.AddTransient<IIdGeneratorRepository, DefaultIdGeneratorRepository>();

            if (config != null)
            {
                var configBuilder = new IdempotencyConfigurationBuilder(services);

                config.Invoke(configBuilder);
            }

            return services;
        }
    }
}
