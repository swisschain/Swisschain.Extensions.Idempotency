using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency.Outbox
{
    public interface IOutboxDispatcher
    {
        Task Send(object command);
        Task Publish(object evt);
    }
}
