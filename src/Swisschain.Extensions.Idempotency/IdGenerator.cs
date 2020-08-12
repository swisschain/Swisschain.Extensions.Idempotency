using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    internal sealed class IdGenerator : IIdGenerator
    {
        readonly IIdGeneratorRepository _idGeneratorRepository;

        public IdGenerator(IIdGeneratorRepository idGeneratorRepository)
        {
            _idGeneratorRepository = idGeneratorRepository;
        }

        public Task<long> GetId(string idempotencyId, string generatorName)
        {
            return _idGeneratorRepository.GetId(idempotencyId, generatorName);
        }
    }
}
