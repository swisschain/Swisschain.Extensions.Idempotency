using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class OutboxManager : IOutboxManager
    {
        private readonly IOutboxDispatcher _dispatcher;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public OutboxManager(IOutboxDispatcher dispatcher, 
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _dispatcher = dispatcher;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public Task<Outbox> Open(string idempotencyId)
        {
            var unitOfWork = _unitOfWorkFactory.Create();

            return unitOfWork.Outbox.Open(unitOfWork, _dispatcher, idempotencyId);
        }
    }
}
