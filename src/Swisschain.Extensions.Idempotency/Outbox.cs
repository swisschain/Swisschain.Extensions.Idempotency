using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Swisschain.Extensions.Idempotency
{
    public sealed class Outbox
    {
        private readonly List<object> _commands = new List<object>();
        private readonly List<object> _events = new List<object>();

        private Outbox(string requestId, long aggregateId, bool isStored, bool isDispatched)
        {
            RequestId = requestId;
            AggregateId = aggregateId;
            IsStored = isStored;
            IsDispatched = isDispatched;
        }

        public string RequestId { get; }
        public long AggregateId { get; }
        public bool IsStored { get; internal set; }
        public bool IsDispatched { get; internal set; }
        public object Response { get; private set; }
        public IReadOnlyCollection<object> Commands => new ReadOnlyCollection<object>(_commands);
        public IReadOnlyCollection<object> Events => new ReadOnlyCollection<object>(_events);

        public void Return<TResponse>(TResponse response)
        {
            if (IsStored)
            {
                throw new InvalidOperationException("Stored outbox can't be modified");
            }
            
            Response = response;
        }

        public TResponse GetResponse<TResponse>()
        {
            return (TResponse) Response;
        }

        public void Send(object command)
        {
            if (IsStored)
            {
                throw new InvalidOperationException("Stored outbox can't be modified");
            }

            _commands.Add(command);
        }

        public void Publish(object evt)
        {
            if (IsStored)
            {
                throw new InvalidOperationException("Stored outbox can't be modified");
            }

            _events.Add(evt);
        }

        public static Outbox Restore(string requestId,
            long aggregateId,
            bool isStored,
            bool isDispatched,
            object response,
            IEnumerable<object> commands,
            IEnumerable<object> events)
        {
            var instance = new Outbox(requestId, aggregateId, isStored, isDispatched)
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
