using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// Valid only for the <see cref="IsTransactional"/> unit of work.
        /// </summary>
        IOutboxWriteRepository OutboxWriteRepository { get; }
        bool IsTransactional { get; }
        bool IsCommitted { get; }
        bool IsRolledBack { get; }
        /// <summary>
        /// Valid only for the <see cref="IsTransactional"/> unit of work.
        /// </summary>
        Outbox Outbox { get; }

        /// <summary>
        /// Initializes transactional unit of work
        /// Used internally by Swisschain.Idempotency infrastructure
        /// </summary>
        Task Init(IOutboxDispatcher defaultOutboxDispatcher,
            IOutboxWriteRepository outboxWriteRepository, 
            Outbox outbox);

        /// <summary>
        /// Closes the <see cref="Outbox"/> and commits unit of work transaction.
        /// Valid only for the <see cref="IsTransactional"/> unit of work.
        /// </summary>
        Task Commit();

        /// <summary>
        /// Rollbacks unit of work transaction.
        /// Valid only for the <see cref="IsTransactional"/> unit of work.
        /// </summary>
        Task Rollback();

        /// <summary>
        /// Dispatches <see cref="Outbox"/> using default <see cref="IOutboxDispatcher"/>.
        /// It's recommended to use more specific dispatcher if possible.
        /// Valid only for the <see cref="IsTransactional"/> unit of work.
        /// </summary>
        Task EnsureOutboxDispatched();

        /// <summary>
        /// Dispatches <see cref="Outbox"/> using specified <paramref name="dispatcher"/>.
        /// Valid only for the <see cref="IsTransactional"/> unit of work.
        /// </summary>
        Task EnsureOutboxDispatched(IOutboxDispatcher dispatcher);
    }
}
