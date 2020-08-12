using System.Threading.Tasks;
using Swisschain.Extensions.Idempotency;

namespace Tests
{
    public class UsageExample
    {
        private readonly IUnitOfWorkManager<ExampleUnitOfWork> _unitOfWorkManager;
        private readonly IIdGenerator _idGenerator;

        public UsageExample(IUnitOfWorkManager<ExampleUnitOfWork> unitOfWorkManager,
            IIdGenerator idGenerator)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _idGenerator = idGenerator;
        }

        public async Task Foo()
        {
            await using var unitOfWork = await _unitOfWorkManager.Begin("a");

            if (!unitOfWork.Outbox.IsClosed)
            {
                var id = await _idGenerator.GetId(unitOfWork.IdempotencyId, "id-generator-account");

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
