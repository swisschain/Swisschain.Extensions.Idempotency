using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IIdGeneratorRepository
    {
        Task<long> GetId(string idempotencyId, string generatorName);
    }
}
