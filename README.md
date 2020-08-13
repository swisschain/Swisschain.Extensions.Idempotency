# Swisschain.Extensions.Idempotency
Extensions to provide idempotency in the services

## Install nuget package

`Install-Package swisschain.extensions.idempotency`

Additional packages are required to use the Outbox dispatching, Outbox persistance, and ID generator persistance. Please read their documentation as well.

### Dispatching packages

- [MassTransit](https://github.com/swisschain/Swisschain.Extensions.Idempotency.MassTransit)

### Persistance packages

- [EfCore](https://github.com/swisschain/Swisschain.Extensions.Idempotency.EfCore)

## Basics

- Unit of work allows you to change several objects atomically. Under the hood it starts and commits/rollbacks a transaction. This means, that underlying persistence provider should support transactions.
- Outbox allows you to "remember" all the outputs of the particular request and produce the same outputs whenever the request is duplicated or retried. Besides it avoids extra output message publication in the case if the messages were already sent for sure.
- Unit of work and outbox together allows you to make outcome (data update, output messages and response) of your method idempotent.
- ID generator allows you to get the same ID for the request using specified ID sequence despite of how many times it's called.

## Initialization

Derive your unit of work from `Swisschain.Extensions.Idempotency.EfCore.UnitOfWorkBase<TDbContext>` class (see [EfCore](https://github.com/swisschain/Swisschain.Extensions.Idempotency.EfCore)).

Call `AddIdempotency` in your DI container initialization, passing your `IUnitOfWork` implementation as a generic parameter:

```c#

services.AddIdempotency<UnitOfWork>(x =>
{
    // Idempotency configuration goes here
});            
```

For example, if you want to use MassTransit dispatching and EF Core persistance, install corresponding packages and configure the idempotency like this:

```c#
services.AddIdempotency<UnitOfWork>(x =>
{
    x.DispatchWithMassTransit();
    x.PersistWithEfCore(s => s.GetRequiredService<DatabaseContext>());
});

```

## Usage

Whenever you need to make your method idempotent, follow this pattern:

```c#
public class TransfersService
{
    IOutboxManager _unitOfWorkManager;
    IIdGenerator _idGenerator;

    // Inject `IOutboxManager` to the service:
    public TransfersService(IUnitOfWorkManager<UnitOfWork> unitOfWorkManager, IIdGenerator idGenerator)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _idGenerator = idGenerator;
    }

    public async Task<ExecuteTransferResponse> Execute(ExecuteTransferRequest request)
    {
        // Begin the unit of work, providing unique idempotency ID.
        await using var unitOfWork = await UnitOfWorkManager.Begin("API:Transfers.Execute:{request.RequestId}");
        
        // Check if the outbox wasn't closed yet to avoid executing the work that was already complete
        if (!unitOfWork.Outbox.IsClosed)
        {
            // Generate ID whenever you need, specifying unique idempotency ID and generator name.
            var transferId = _idGenerator.GetId("Transfers:Id:{request.RequestId}", "id_generator_transfers");
        
            // Do update your models here, call service and update the state whatever you need.
            var transfer = Transfer.Accept(
                transferId,
                request.TenantId,
                request.AssetId,
                request.Movements);

            // Use repositories via the unit of work
            await unitOfWork.Transfers.Add(transfer);
            
            var response = new ExecuteTransferResponse
            {
                Id = transfer.Id,
                State = transfer.State,
                AssetId = transfer.AssetId,
                Movements = transfer.Movements
            };

            // If your method has a return value, "return" it to the outbox:
            unitOfWork.Outbox.Return(response);
            
            // If you method need to send commands, "send" them to the outbox:
            unitOfWork.Outbox.Send(new ExecuteTransfer {TransferId = transfer.Id});
            
            // If you method need to publish events, "publish" them to the outbox:
            unitOfWork.Outbox.Publish(new TransferAccepted {TransferId = transfer.Id, RequestId = RequestId});
            
            // In the end of this block, commit the unit of work.
            // You can Rollback it too if you logic requires this. This will rollback all the changes within the outbox.
            // If you missed both Rollback and Commit, the unit of work will be rolled back on disposing.
            await unitOfWork.Commit();
        }
        
        // Dispatch all the messages stored in the outbox (you can omit this, if your method doesn't produce messages):
        // If you use unit of work inside Mass Transit message handler, use overload which accepts ConsumeContext to improve
        // messages traceability
        await unitOfWork.EnsureOutboxDispatched();
        
        // And return the response stored in the outbox:
        return outbox.GetResponse<ExecuteTransferResponse>();
    }
}

```
