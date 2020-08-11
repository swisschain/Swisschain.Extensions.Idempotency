using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IOutboxRepository
    {
        Task<Outbox> GetOrDefault(IUnitOfWork unitOfWork, IOutboxDispatcher dispatcher, string idempotencyId);
        Task Save(Outbox outbox);
    }
}
