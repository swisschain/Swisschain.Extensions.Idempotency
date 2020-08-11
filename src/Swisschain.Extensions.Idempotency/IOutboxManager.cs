using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IOutboxManager
    {
        Task<Outbox> Open(string idempotencyId);
    }
}
