# Swisschain.Extensions.Idempotency
Extensions to provide idempotency in the services

## Install nuget package

`Install-Package swisschain.extensions.idempotency`

Additional packages are required to use the Outbox dispatching and persistance.

### Dispatching packages

- [MassTransit](https://github.com/swisschain/Swisschain.Extensions.Idempotency.MassTransit)

### Persistance packages

- [EfCore](https://github.com/swisschain/Swisschain.Extensions.Idempotency.EfCore)

## Initialization:


```c#
services.AddOutbox(c =>
{
    // Outbox configuration goes here
});
            
```

For example, if you want to use the Outbox with MassTransit dispatching and EF Core persistance, install corresponding packages and configure the outbox like this:

```c#
services.AddOutbox(c =>
{
    c.DispatchWithMassTransit();
    c.PersistWithEfCore(s =>
    {
        var optionsBuilder = s.GetRequiredService<DbContextOptionsBuilder<DatabaseContext>>();

        return new DatabaseContext(optionsBuilder.Options);
    });
});
    
```

## Usage

Whenever you need to make your method idempotent, follow this pattern:

```c#
public class TransfersService
{
    IOutboxManager _outbox;
    ITransfersRepository _repository;

    // Inject `IOutboxManager` to the service:
    public TransfersService(IOutboxManager outbox, ITransfersRepository repository)
    {
        _outbox = outbox;
        _repository = repository;
    }

    public async Task<ExecuteTransferResponse> Execute(ExecuteTransferRequest request)
    {
        // Open the outbox, providing unique idempotency request ID and optionally factory of the aggregate ID which is created by this method.
        var outbox = await _outbox.Open($"Transfers_Execute_{request.RequestId}", () => _transfersRepository.GetIdAsync());
        
        // Check if the outbox wasn't stored yet
        if (!outbox.IsStored)
        {
            // Do update your models here, call service and update the state whatever you need. 
            // But take into account, that each step should be idempotent itself.
            var transfer = Transfer.Accept(
                outbox.AggregateId,
                request.TenantId,
                request.AssetId,
                request.Movements);

            await _transfersRepository.AddOrIgnoreAsync(transfer);
            
            var response = new ExecuteTransferResponse
            {
                Id = transfer.Id,
                State = transfer.State,
                AssetId = transfer.AssetId,
                Movements = transfer.Movements
            };

            // If your method has a return value, "return" it to the outbox:
            outbox.Return(response);
            
            // If you method need to send commands, "send" them to the outbox:
            outbox.Send(new ExecuteTransfer {TransferId = transfer.Id});
            
            // If you method need to publish events, "publish" them to the outbox:
            outbox.Publish(new TransferAccepted {TransferId = transfer.Id, RequestId = RequestId});
            
            // In the end of this block, store the outbox
            await _outbox.Store(outbox);
        }
        
        // Dispatch all the messages stored in the outbox (you can omit this, if your method doesn't produce messages):
        await _outbox.EnsureDispatched(outbox);
        
        // And return the response stored in the outbox:
        return outbox.GetResponse<ExecuteTransferResponse>();
    }
}

```