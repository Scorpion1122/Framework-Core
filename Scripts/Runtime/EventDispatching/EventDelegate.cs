
using System;

namespace Thijs.Core.EventDispatching
{
    public abstract class EventDelegate
    {
        public object Target { get; private set; }

        public EventDelegate(object target)
        {
            Target = target;
        }

        public abstract void Invoke(IEventBase eventBae);
    }
    
    public class EventDelegate<T> : EventDelegate where T : IEventBase
    {
        private Action<T> action;

        public EventDelegate(object target, Action<T> action)
            : base(target)
        {
            this.action = action;
        }

        public override void Invoke(IEventBase eventBae)
        {
            action((T)eventBae);
        }
    }
}
