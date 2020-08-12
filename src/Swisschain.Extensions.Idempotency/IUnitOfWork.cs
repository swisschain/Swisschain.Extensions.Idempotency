using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IOutboxWriteRepository OutboxWriteRepository { get; }
        string IdempotencyId { get; }
        bool IsCommitted { get; }
        bool IsRolledBack { get; }
        Outbox Outbox { get; }

        Task Init(IOutboxWriteRepository outboxWriteRepository,
            string idempotencyId,
            Outbox outbox);

        /// <summary>
        /// Closes the <see cref="Outbox"/> and commits unit of work transaction
        /// </summary>
        Task Commit();

        /// <summary>
        /// Rollbacks unit of work transaction
        /// </summary>
        Task Rollback();

        /// <summary>
        /// Dispatches <see cref="Outbox"/> using default <see cref="IOutboxDispatcher"/>.
        /// It's recommended to use more specific dispatcher if possible.
        /// </summary>
        Task EnsureOutboxDispatched();

        /// <summary>
        /// Dispatches <see cref="Outbox"/> using specified <paramref name="dispatcher"/>.
        /// </summary>
        Task EnsureOutboxDispatched(IOutboxDispatcher dispatcher);
    }
}
