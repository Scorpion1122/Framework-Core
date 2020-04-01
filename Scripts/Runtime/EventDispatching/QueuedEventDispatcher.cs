using System;
using System.Collections.Generic;
using UnityEngine;

namespace TKO.Core.EventDispatching
{
    public class QueuedEventDispatcher : IEventDispatcher
    {
        public string Name { get; set; }

        protected Dictionary<Type, List<EventDelegate>> mapping;
        protected Queue<DispatchCommand> dispatchQueue;

        public QueuedEventDispatcher()
        {
            mapping = new Dictionary<Type, List<EventDelegate>>();
            dispatchQueue = new Queue<DispatchCommand>();
        }

        public virtual void Dispatch<T>(object sender, T data) where T : IEventBase
        {
            DispatchCommand command = new DispatchCommand()
            {
                sender = sender,
                data = data
            };
            dispatchQueue.Enqueue(command);

            if (dispatchQueue.Count == 1)
                SendQueuedEvent();
        }

        protected virtual void SendQueuedEvent()
        {
            if (dispatchQueue.Count != 0)
            {
                SendEvent(dispatchQueue.Peek());
                dispatchQueue.Dequeue();
                SendQueuedEvent();
            }
        }

        protected virtual void SendEvent(DispatchCommand command)
        {
            if (mapping.ContainsKey(command.DataType))
                SendEvent(command, mapping[command.DataType]);
        }

        protected virtual void SendEvent(DispatchCommand command, List<EventDelegate> handlers)
        {
            for (int i = handlers.Count; i >= 0; i--)
            {
                if (i >= handlers.Count)
                    continue;
                SendEvent(command, handlers[i]);
            }
        }

        protected virtual void SendEvent(DispatchCommand command, EventDelegate handeler)
        {
            handeler.Invoke(command.data);
        }

        public virtual void Listen<T>(Action<T> handler) where T : IEventBase
        {
            Type type = typeof(T);
            if (!mapping.ContainsKey(type))
                CreateNewEntry(type);

            mapping[type].Add(new EventDelegate<T>(handler.Target, handler));
        }

        protected virtual void CreateNewEntry(Type type)
        {
            mapping.Add(type, new List<EventDelegate>());
        }

        public virtual void Release<T>(object listener) where T : IEventBase
        {
            Type type = typeof(T);
            if (mapping.ContainsKey(type))
            {
                List<EventDelegate> delegates = mapping[type];
                for (int i = 0; i < delegates.Count; i++)
                {
                    if (delegates[i].Target == listener)
                    {
                        delegates.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public virtual void ReleaseAll(object listener)
        {
            foreach (List<EventDelegate> delegates in mapping.Values)
            {
                for (int i = 0; i < delegates.Count; i++)
                {
                    if (delegates[i].Target == listener)
                    {
                        delegates.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public virtual void ReleaseAll()
        {
            mapping.Clear();
        }
    }
}
