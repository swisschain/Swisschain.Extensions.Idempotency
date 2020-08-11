using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IIdGeneratorRepository
    {
        Task<long> GetId(string idempotencyId, Func<Task<long>> idFactory);
    }
}
