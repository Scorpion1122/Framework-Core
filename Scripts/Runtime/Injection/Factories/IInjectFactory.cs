using System;

namespace TKO.Core.Injection
{
    public interface IInjectFactory
    {
        bool AtEditTime { get; }
        void InjectInto(Type type, object target);
        bool HasRequiredAttributes(Type type);
    }
}