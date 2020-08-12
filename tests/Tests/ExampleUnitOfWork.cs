using System.Threading.Tasks;
using Swisschain.Extensions.Idempotency;

namespace Tests
{
    public class ExampleUnitOfWork : UnitOfWorkBase
    {
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
