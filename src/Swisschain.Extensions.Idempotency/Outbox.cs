using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Swisschain.Extensions.Idempotency
{
    public sealed class Outbox
    {
        private readonly List<object> _commands = new List<object>();
        private readonly List<object> _events = new List<object>();

        private Outbox(string idempotencyId, 
            bool isClosed,
            bool isDispatched)
        {
            IdempotencyId = idempotencyId;
            IsClosed = isClosed;
            IsDispatched = isDispatched;
        }

        public string IdempotencyId { get; }
        public bool IsClosed { get; private set; }
        public bool IsDispatched { get; private set; }
        public object Response { get; private set; }
        public IReadOnlyCollection<object> Commands => new ReadOnlyCollection<object>(_commands);
        public IReadOnlyCollection<object> Events => new ReadOnlyCollection<object>(_events);

        internal void Close()
        {
            if (IsClosed)
            {
                throw new InvalidOperationException($"The outbox {IdempotencyId} is already closed");
            }

            IsClosed = true;
        }

        internal async Task EnsureDispatched(IOutboxDispatcher dispatcher)
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
        }

        public void Return<TResponse>(TResponse response)
        {
            if (IsClosed)
            {
                throw new InvalidOperationException($"Closed outbox {IdempotencyId} can't be modified");
            }
            
            Response = response;
        }

        public TResponse GetResponse<TResponse>()
        {
            if (!IsClosed)
            {
                throw new InvalidOperationException($"The outbox {IdempotencyId} is not closed yet, so its response can't be obtained");
            }

            return (TResponse) Response;
        }

        public void Send(object command)
        {
            if (IsClosed)
            {
                throw new InvalidOperationException($"Closed outbox {IdempotencyId} can't be modified");
            }

            _commands.Add(command);
        }

        public void Publish(object evt)
        {
            if (IsClosed)
            {
                throw new InvalidOperationException($"Closed outbox {IdempotencyId} can't be modified");
            }

            _events.Add(evt);
        }

        public static Outbox Open(string idempotencyId)
        {
            return new Outbox(
                idempotencyId,
                isClosed: false,
                isDispatched: false);
        }

        public static Outbox Restore(string idempotencyId,
            bool isDispatched,
            object response,
            IEnumerable<object> commands,
            IEnumerable<object> events)
        {
            var instance = new Outbox(idempotencyId, isClosed: true, isDispatched)
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
    }
}
