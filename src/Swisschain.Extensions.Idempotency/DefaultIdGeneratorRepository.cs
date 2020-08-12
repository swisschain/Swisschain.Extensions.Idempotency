using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class DefaultIdGeneratorRepository : IIdGeneratorRepository
    {
        public Task<long> GetId(string idempotencyId, string generatorName)
        {
            throw new InvalidOperationException("ID generator repository is not configured. To use ID generator, you need to configure a persistence in service.AddIdempotency(c => {...})");
        }
    }
}
