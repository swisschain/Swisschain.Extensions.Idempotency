using Microsoft.Extensions.DependencyInjection;

namespace Swisschain.Extensions.Idempotency
{
    public sealed class IdempotencyConfigurationBuilder
    {
        internal IdempotencyConfigurationBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
