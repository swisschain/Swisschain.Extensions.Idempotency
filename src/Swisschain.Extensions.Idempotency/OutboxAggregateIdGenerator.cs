using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public static class OutboxAggregateIdGenerator
    {
        public static readonly Func<Task<long>> None = () => Task.FromResult(0L);
        public static readonly Func<Task<long>> Sequential = () => Task.FromResult(0L);
    }
}
