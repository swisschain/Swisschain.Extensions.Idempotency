using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        private readonly IOutboxDispatcher _defaultOutboxDispatcher;

        public UnitOfWorkBase(IOutboxDispatcher defaultOutboxDispatcher)
        {
            _defaultOutboxDispatcher = defaultOutboxDispatcher;
        }

        public IOutboxWriteRepository OutboxWriteRepository { get; private set; }
        public bool IsCommitted { get; private set; }
        public bool IsRolledBack { get; private set; }
        public Outbox Outbox { get; private set; }

        protected abstract Task CommitImpl();
        protected abstract Task RollbackImpl();
        protected abstract ValueTask DisposeAsync(bool disposing);

        public Task Init(IOutboxWriteRepository outboxWriteRepository,
            Outbox outbox)
        {
            OutboxWriteRepository = outboxWriteRepository;
            Outbox = outbox;

            return Task.CompletedTask;
        }

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
