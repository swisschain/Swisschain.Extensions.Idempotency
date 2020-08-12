using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class UnitOfWorkManager<TUnitOfWork> : IUnitOfWorkManager<TUnitOfWork> 
        where TUnitOfWork : UnitOfWorkBase
    {
        private readonly IUnitOfWorkFactory<TUnitOfWork> _unitOfWorkFactory;
        private readonly IOutboxManager _outboxManager;

        public UnitOfWorkManager(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, IOutboxManager outboxManager)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _outboxManager = outboxManager;
        }

        public async Task<TUnitOfWork> Begin(string idempotencyId)
        {
            var outbox = await _outboxManager.Open(idempotencyId);
            var unitOfWork = await _unitOfWorkFactory.Create(outbox);

            return unitOfWork;
        }
    }
}
