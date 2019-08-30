using System;

namespace Thijs.Core.EventDispatching
{
    public struct DispatchCommand
    {
        public Type DataType => data.GetType();
        
        public object sender;
        public IEventBase data;
    }
}
