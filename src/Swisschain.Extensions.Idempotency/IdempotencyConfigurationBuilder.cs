using Microsoft.Extensions.DependencyInjection;

namespace Swisschain.Extensions.Idempotency
{
    public sealed class IdempotencyConfigurationBuilder<TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase
    {
        internal IdempotencyConfigurationBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
