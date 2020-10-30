using System;

namespace TKO.Core.EventDispatching
{
    public struct DispatchCommand
    {
        public Type DataType => data.GetType();
        
        public object sender;
        public IEventBase data;
    }
}
