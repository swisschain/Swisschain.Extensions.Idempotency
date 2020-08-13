using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IUnitOfWorkManager<TUnitOfWork> 
        where TUnitOfWork : UnitOfWorkBase
    {
        Task<TUnitOfWork> Begin(string idempotencyId);
    }
}
