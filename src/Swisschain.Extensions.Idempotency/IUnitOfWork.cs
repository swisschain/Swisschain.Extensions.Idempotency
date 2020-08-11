using System;

namespace Swisschain.Extensions.Idempotency
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Should be used only by Swisschain.Extensions.Idempotency
        /// </summary>
        IOutboxRepository Outbox { get; }

        /// <summary>
        /// Should be used only by Swisschain.Extensions.Idempotency
        /// </summary>
        void Commit();

        /// <summary>
        /// Should be used only by Swisschain.Extensions.Idempotency
        /// </summary>
        void Rollback();
    }
}