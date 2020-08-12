using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class OutboxManager : IOutboxManager
    {
        private readonly IOutboxReadRepository _repository;

        public OutboxManager(IOutboxReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Outbox> Open(string idempotencyId)
        {
            var existingOutbox = await _repository.GetOrDefault(idempotencyId);

            return existingOutbox ?? Outbox.Open(idempotencyId);
        }
    }
}
