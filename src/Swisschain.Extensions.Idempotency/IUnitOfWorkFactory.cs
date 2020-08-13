using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IUnitOfWorkFactory<TUnitOfWork>
        where TUnitOfWork : UnitOfWorkBase
    {
        Task<TUnitOfWork> Create(Outbox outbox);
    }
}
