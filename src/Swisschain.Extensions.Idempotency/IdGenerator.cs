using System;
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
        
        public Task<long> GetId(string idempotencyId, Func<Task<long>> idFactory)
        {
            return _repository.GetId(idempotencyId, idFactory);
        }

        public Task<long> GetId(string idempotencyId)
        {
            return GetId(idempotencyId, IdFactory.Sequential);
        }
    }
}
