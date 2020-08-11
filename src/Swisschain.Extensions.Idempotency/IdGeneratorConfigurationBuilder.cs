using Microsoft.Extensions.DependencyInjection;

namespace Swisschain.Extensions.Idempotency
{
    public sealed class IdGeneratorConfigurationBuilder
    {
        internal IdGeneratorConfigurationBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
