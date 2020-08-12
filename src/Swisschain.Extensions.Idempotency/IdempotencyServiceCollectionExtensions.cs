using System;
using Microsoft.Extensions.DependencyInjection;

namespace Swisschain.Extensions.Idempotency
{
    public static class IdempotencyServiceCollectionExtensions
    {
        public static IServiceCollection AddIdempotency<TUnitOfWork>(this IServiceCollection services, Action<IdempotencyConfigurationBuilder<TUnitOfWork>> config)
            where TUnitOfWork : UnitOfWorkBase
        {
            services.AddTransient<IUnitOfWorkManager<TUnitOfWork>, UnitOfWorkManager<TUnitOfWork>>();

            services.AddTransient<IOutboxManager, OutboxManager>();
            services.AddTransient<IIdGenerator, IdGenerator>();

            services.AddTransient<IOutboxDispatcher, DefaultOutboxDispatcher>();

            services.AddTransient<IOutboxReadRepository, DefaultOutboxReadRepository>();
            services.AddTransient<IIdGeneratorRepository, DefaultIdGeneratorRepository>();

            if (config != null)
            {
                var configBuilder = new IdempotencyConfigurationBuilder<TUnitOfWork>(services);

                config.Invoke(configBuilder);
            }

            return services;
        }
    }
}
