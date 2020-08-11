using System;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public static class IdFactory
    {
        public static readonly Func<Task<long>> Sequential = () => Task.FromResult(0L);
    }
}
