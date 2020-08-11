using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class IdGenerator : IIdGenerator
    {
        private readonly IIdGeneratorRepository _repository;

        public IdGenerator(IIdGeneratorRepository repository)
        {
            _repository = repository;
        }
        
        public Task<long> GetId(string idempotencyId, string generatorName)
        {
            return _repository.GetId(idempotencyId, generatorName);
        }
    }
}
