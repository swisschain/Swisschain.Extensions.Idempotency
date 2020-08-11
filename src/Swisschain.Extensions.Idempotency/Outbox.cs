using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public sealed class Outbox : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxDispatcher _dispatcher;
        private readonly List<object> _commands = new List<object>();
        private readonly List<object> _events = new List<object>();

        private Outbox(IUnitOfWork unitOfWork, 
            IOutboxDispatcher dispatcher, 
            string idempotencyId, 
            bool isClosed,
            bool isDispatched)
        {
            _unitOfWork = unitOfWork;
            _dispatcher = dispatcher;
            IdempotencyId = idempotencyId;
            IsClosed = isClosed;
            IsDispatched = isDispatched;
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (IsClosed)
                {
                    throw new InvalidOperationException("Unit of work can be obtained only if the outbox is opened");
                }

                return _unitOfWork;
            }
        }

        public string IdempotencyId { get; }
        public bool IsClosed { get; private set; }
        public bool IsDispatched { get; private set; }
        public object Response { get; private set; }
        public IReadOnlyCollection<object> Commands => new ReadOnlyCollection<object>(_commands);
        public IReadOnlyCollection<object> Events => new ReadOnlyCollection<object>(_events);

        public void Close()
        {
            if (IsClosed)
            {
                throw new InvalidOperationException($"The outbox {IdempotencyId} is already closed");
            }

            IsClosed = true;

            _unitOfWork.Outbox.Save(this);
            _unitOfWork.Commit();
        }

        
        public Task EnsureDispatched()
        {
            return EnsureDispatched(_dispatcher);
        }

        public async Task EnsureDispatched(IOutboxDispatcher dispatcher)
        {
            if (!IsClosed)
            {
                throw new InvalidOperationException($"The outbox {IdempotencyId} is not closed yet, so it can't be dispatched");
            }

            if (IsDispatched)
            {
                return;
            }

            var dispatchingTasks = new List<Task>();

            foreach (var command in Commands)
            {
                dispatchingTasks.Add(dispatcher.Send(command));
            }

            foreach (var evt in Events)
            {
                dispatchingTasks.Add(dispatcher.Publish(evt));
            }

            await Task.WhenAll(dispatchingTasks);

            IsDispatched = true;

            await _unitOfWork.Outbox.Save(this);
        }

        public void Return<TResponse>(TResponse response)
        {
            if (IsClosed)
            {
                throw new InvalidOperationException("Closed outbox can't be modified");
            }
            
            Response = response;
        }

        public TResponse GetResponse<TResponse>()
        {
            return (TResponse) Response;
        }

        public void Send(object command)
        {
            if (IsClosed)
            {
                throw new InvalidOperationException("Closed outbox can't be modified");
            }

            _commands.Add(command);
        }

        public void Publish(object evt)
        {
            if (IsClosed)
            {
                throw new InvalidOperationException("Closed outbox can't be modified");
            }

            _events.Add(evt);
        }

        public static Outbox Restore(IUnitOfWork unitOfWork,
            IOutboxDispatcher dispatcher,
            string idempotencyId,
            bool isClosed,
            bool isDispatched,
            object response,
            IEnumerable<object> commands,
            IEnumerable<object> events)
        {
            var instance = new Outbox(unitOfWork, dispatcher, idempotencyId, isClosed, isDispatched)
            {
                Response = response
            };

            if (commands != null)
            {
                instance._commands.AddRange(commands);
            }

            if (events != null)
            {
                instance._events.AddRange(events);
            }

            return instance;
        }

        public void Dispose()
        {
            if (!IsClosed)
            {
                _unitOfWork.Rollback();
            }

            _unitOfWork.Dispose();
        }
    }
}
