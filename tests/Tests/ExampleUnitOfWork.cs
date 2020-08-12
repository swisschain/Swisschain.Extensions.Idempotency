using System.Threading.Tasks;
using Swisschain.Extensions.Idempotency;

namespace Tests
{
    public class ExampleUnitOfWork : UnitOfWorkBase
    {
        public ExampleUnitOfWork(IOutboxWriteRepository outboxWriteRepository,
            IIdGeneratorRepository idGeneratorRepository,
            string idempotencyId,
            Outbox outbox,
            IOutboxDispatcher defaultOutboxDispatcher) : 

            base(outboxWriteRepository, 
                idGeneratorRepository, 
                idempotencyId,
                outbox,
                defaultOutboxDispatcher)
        {
        }

        protected override Task CommitImpl()
        {
            throw new System.NotImplementedException();
        }

        protected override Task RollbackImpl()
        {
            throw new System.NotImplementedException();
        }

        protected override ValueTask DisposeAsync(bool disposing)
        {
            throw new System.NotImplementedException();
        }
    }
}
