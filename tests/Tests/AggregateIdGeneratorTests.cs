using System;
using System.Threading.Tasks;
using Shouldly;
using Swisschain.Extensions.Idempotency;
using Xunit;

namespace Tests
{
    public class AggregateIdGeneratorTests
    {
        [Fact]
        public void CanRecognizeAggregateIdGenerator()
        {
            var myGenerator = new Func<Task<long>>(() => Task.FromResult(123L));

            myGenerator.ShouldNotBe(OutboxAggregateIdGenerator.None);
            myGenerator.ShouldNotBe(OutboxAggregateIdGenerator.Sequential);

            OutboxAggregateIdGenerator.None.ShouldNotBe(OutboxAggregateIdGenerator.Sequential);

            OutboxAggregateIdGenerator.None.ShouldBe(OutboxAggregateIdGenerator.None);
            OutboxAggregateIdGenerator.Sequential.ShouldBe(OutboxAggregateIdGenerator.Sequential);
        }
    }
}
