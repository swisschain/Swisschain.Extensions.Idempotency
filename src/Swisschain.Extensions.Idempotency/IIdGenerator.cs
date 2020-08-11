using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IIdGenerator
    {
        /// <summary>
        /// Generates new ID using the specified <paramref name="generatorName"/> for the given
        /// <paramref name="idempotencyId"/> or returns existing ID if any.
        /// </summary>
        Task<long> GetId(string idempotencyId, string generatorName);
    }
}
