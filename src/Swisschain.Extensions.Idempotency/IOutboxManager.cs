using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal interface IOutboxManager
    {
        Task<Outbox> Open(string idempotencyId);
    }
}
