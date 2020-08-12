using System.Threading.Tasks;
using Swisschain.Extensions.Idempotency;

namespace Tests
{
    public class UsageExample
    {
        private readonly IUnitOfWorkManager<ExampleUnitOfWork> _unitOfWorkManager;

        public UsageExample(IUnitOfWorkManager<ExampleUnitOfWork> unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task Foo()
        {
            await using var unitOfWork = await _unitOfWorkManager.Begin("a");

            if (!unitOfWork.Outbox.IsClosed)
            {
                var id = await unitOfWork.GenerateId("id-generator-account");

                // DB state updated here

                unitOfWork.Outbox.Publish(new object());
                unitOfWork.Outbox.Send(new object());
                unitOfWork.Outbox.Return(new object());
                
                await unitOfWork.Commit();
            }

            await unitOfWork.EnsureOutboxDispatched();
        }
    }
}
