﻿using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IIdGenerator
    {
        Task<long> GetId(string idempotencyId, string generatorName);
    }
}