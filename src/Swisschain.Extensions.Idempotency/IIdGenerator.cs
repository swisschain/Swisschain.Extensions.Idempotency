using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IIdGenerator
    {
        /// <summary>
        /// Generates new ID for the given <paramref name="idempotencyId"/> or
        /// returns existing ID if any, using provided <paramref name="idFactory"/>
        /// </summary>
        Task<long> GetId(string idempotencyId, Func<Task<long>> idFactory);

        /// <summary>
        /// Generates new ID for the given <paramref name="idempotencyId"/> or
        /// returns existing ID if any, using built-in sequence.
        /// </summary>
        Task<long> GetId(string idempotencyId);
    }
}
