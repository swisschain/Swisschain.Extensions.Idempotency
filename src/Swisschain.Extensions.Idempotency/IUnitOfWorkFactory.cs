namespace Swisschain.Extensions.Idempotency
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}