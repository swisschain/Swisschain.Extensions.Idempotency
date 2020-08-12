using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IIdGeneratorRepository IdGeneratorRepository { get; }
        IOutboxWriteRepository OutboxWriteRepository { get; }
        string IdempotencyId { get; }
        bool IsCommitted { get; }
        bool IsRolledBack { get; }
        Outbox Outbox { get; }

        Task Init(IIdGeneratorRepository idGeneratorRepository,
            IOutboxWriteRepository outboxWriteRepository,
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
        /// Generates or returns existing unique ID for the given <see cref="IdempotencyId"/> using
        /// <see cref="IdGeneratorRepository"/> and specified <paramref name="generatorName"/>
        /// </summary>
        Task<long> GenerateId(string generatorName);

        /// <summary>
        /// Generates or returns existing unique ID for the specified <paramref name="idempotencyId"/> using
        /// <see cref="IdGeneratorRepository"/> and specified <paramref name="generatorName"/>
        /// </summary>
        Task<long> GenerateId(string idempotencyId, string generatorName);

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
