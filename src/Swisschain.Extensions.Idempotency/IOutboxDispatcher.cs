using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public interface IOutboxDispatcher
    {
        Task Send(object command);
        Task Publish(object evt);
    }
}
