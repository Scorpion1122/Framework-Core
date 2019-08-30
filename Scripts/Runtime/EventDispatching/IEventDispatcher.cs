using System;

namespace Thijs.Core.EventDispatching
{
    public interface IEventDispatcher
    {
        string Name { get; set; }

        void Dispatch<T>(object sender, T data) where T : IEventBase;

        void Listen<T>(Action<T> handeler) where T : IEventBase;

        void Release<T>(object listener) where T : IEventBase;
        void ReleaseAll(object listener);
        void ReleaseAll();
    }
}
