using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IUnitOfWorkManager<TUnitOfWork> 
        where TUnitOfWork : UnitOfWorkBase
    {
        /// <summary>
        /// Begins the transactional unit of work
        /// </summary>
        Task<TUnitOfWork> Begin(string idempotencyId);

        /// <summary>
        /// Begins either non-transactional unit of work
        /// </summary>
        Task<TUnitOfWork> Begin();
    }
}
