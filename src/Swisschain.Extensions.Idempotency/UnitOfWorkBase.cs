using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        private IOutboxDispatcher _defaultOutboxDispatcher;
        private Outbox _outbox;
        private IOutboxWriteRepository _outboxWriteRepository;

        public IOutboxWriteRepository OutboxWriteRepository
        {
            get
            {
                if (!IsTransactional)
                {
                    throw new InvalidOperationException("Outbox repository of the non-transactional unit of work can't be accessed");
                }

                return _outboxWriteRepository;
            }
            private set => _outboxWriteRepository = value;
        }

        public bool IsTransactional => Outbox != null;
        public bool IsCommitted { get; private set; }
        public bool IsRolledBack { get; private set; }

        public Outbox Outbox
        {
            get
            {
                if (!IsTransactional)
                {
                    throw new InvalidOperationException("Outbox of the non-transactional unit of work can't be accessed");
                }

                return _outbox;
            }
            private set => _outbox = value;
        }

        protected abstract Task CommitImpl();
        protected abstract Task RollbackImpl();
        protected abstract ValueTask DisposeAsync(bool disposing);

        public Task Init(IOutboxDispatcher defaultOutboxDispatcher,
            IOutboxWriteRepository outboxWriteRepository,
            Outbox outbox)
        {
            _defaultOutboxDispatcher = defaultOutboxDispatcher;

            OutboxWriteRepository = outboxWriteRepository;
            Outbox = outbox;

            return Task.CompletedTask;
        }

        public async Task Commit()
        {
            if (!IsTransactional)
            {
                throw new InvalidOperationException("Non-transactional unit of work can't be committed");
            }

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
            if (!IsTransactional)
            {
                throw new InvalidOperationException("Non-transactional unit of work can't be rolled back");
            }

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
            if (!IsTransactional)
            {
                throw new InvalidOperationException("Outbox of the non-transactional unit of work can't be dispatched");
            }

            await Outbox.EnsureDispatched(dispatcher);
            await OutboxWriteRepository.Update(Outbox);
        }

        public async ValueTask DisposeAsync()
        {
            if (IsTransactional && !(IsCommitted || IsRolledBack))
            {
                await Rollback();
            }

            await DisposeAsync(true);
        }
    }
}
