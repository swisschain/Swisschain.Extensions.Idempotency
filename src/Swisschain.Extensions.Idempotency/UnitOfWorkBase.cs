using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        private readonly IOutboxDispatcher _defaultOutboxDispatcher;

        public UnitOfWorkBase(
            IOutboxWriteRepository outboxWriteRepository,
            IIdGeneratorRepository idGeneratorRepository,
            string idempotencyId,
            Outbox outbox,
            IOutboxDispatcher defaultOutboxDispatcher)
        {
            _defaultOutboxDispatcher = defaultOutboxDispatcher;
            OutboxWriteRepository = outboxWriteRepository;
            IdGeneratorRepository = idGeneratorRepository;
            IdempotencyId = idempotencyId;
            Outbox = outbox;
        }

        public IIdGeneratorRepository IdGeneratorRepository { get; }
        public IOutboxWriteRepository OutboxWriteRepository { get; }
        public string IdempotencyId { get; }
        public bool IsCommitted { get; private set; }
        public bool IsRolledBack { get; private set; }
        public Outbox Outbox { get; }

        protected abstract Task CommitImpl();
        protected abstract Task RollbackImpl();
        protected abstract ValueTask DisposeAsync(bool disposing);

        public async Task Commit()
        {
            if (IsCommitted)
            {
                throw new InvalidOperationException("The unit of work was commit already");
            }

            if (IsRolledBack)
            {
                throw new InvalidOperationException("The unit of work was commit already");
            }

            Outbox.Close();

            await OutboxWriteRepository.Add(Outbox);
            await CommitImpl();

            IsCommitted = true;
        }

        public async Task Rollback()
        {
            if (IsCommitted)
            {
                throw new InvalidOperationException("The unit of work was commit already");
            }

            if (IsRolledBack)
            {
                throw new InvalidOperationException("The unit of work was commit already");
            }

            await RollbackImpl();

            IsRolledBack = true;
        }

        public Task<long> GenerateId(string generatorName)
        {
            return IdGeneratorRepository.GetId(IdempotencyId, generatorName);
        }

        public Task<long> GenerateId(string idempotencyId, string generatorName)
        {
            return IdGeneratorRepository.GetId(idempotencyId, generatorName);
        }

        public Task EnsureOutboxDispatched()
        {
            return EnsureOutboxDispatched(_defaultOutboxDispatcher);
        }

        public async Task EnsureOutboxDispatched(IOutboxDispatcher dispatcher)
        {
            await Outbox.EnsureDispatched(dispatcher);
            await OutboxWriteRepository.Update(Outbox);
        }

        public async ValueTask DisposeAsync()
        {
            if (!(IsCommitted || IsRolledBack))
            {
                await Rollback();
            }

            await DisposeAsync(true);
        }
    }
}
