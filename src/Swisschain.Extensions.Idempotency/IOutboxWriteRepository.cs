using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    /// <summary>
    /// Should work in the context of the <see cref="IUnitOfWork"/> transaction
    /// </summary>
    public interface IOutboxWriteRepository
    {
        Task Add(Outbox outbox);
        Task Update(Outbox outbox);
    }
}
